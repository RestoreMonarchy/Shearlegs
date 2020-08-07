using Autofac;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Delegates;
using Shearlegs.Core.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Shearlegs.Core.Plugins
{
    public class PluginManager : IPluginManager
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly ILogger logger;

        private readonly List<Assembly> loadedPlugins;
        private readonly List<IPlugin> activatedPlugins;

        public event PluginActivated OnPluginActivated;
        public event PluginDeactivated OnPluginDeactivated;

        public IEnumerable<Assembly> LoadedPlugins => loadedPlugins;
        public IEnumerable<IPlugin> ActivatedPlugins => activatedPlugins;

        public PluginManager(ILifetimeScope lifetimeScope, ILogger logger)
        {
            this.lifetimeScope = lifetimeScope;
            this.logger = logger;
            loadedPlugins = new List<Assembly>();
            activatedPlugins = new List<IPlugin>();
        }

        private object LoadConfigurationInstance(Type configurationType, Type pluginType)
        {
            return typeof(PluginHelper)
                .GetMethod(nameof(PluginHelper.ReadPluginConfiguration), BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(configurationType)
                .Invoke(null, new object[] { DirectoryConstants.PluginConfigurationFile(pluginType.Name) });
        }

        public async Task<IPlugin> ActivatePluginAsync(Assembly assembly)
        {            
            var pluginType = assembly.GetTypes().FirstOrDefault(x => x.GetInterface(nameof(IPlugin)) != null);

            if (pluginType == null)
            {
                await logger.LogInformationAsync($"{assembly.GetName().Name} is not valid plugin assembly!");
                return null;
            }

            var configurationType = assembly.GetTypes()
                .FirstOrDefault(x => x.GetCustomAttribute<ConfigurationAttribute>()?.PluginType?.Equals(pluginType) ?? false);

            var defaultTranslations = pluginType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(x => x.GetCustomAttribute<DefaultTranslationsAttribute>() != null)?
                .GetValue(null) as IDictionary<string, string> ?? new Dictionary<string, string>();

            Directory.CreateDirectory(DirectoryConstants.PluginDirectory(pluginType.Name));

            // Load plugin configuration
            object configuration = null;

            if (configurationType != null)
            {
                try
                {
                    configuration = LoadConfigurationInstance(configurationType, pluginType);
                }
                catch (Exception e)
                {
                    await logger.LogExceptionAsync(e, $"Failed to load {pluginType.Name} configuration!");
                    return null;
                }
            }

            // Load plugin translations
            IDictionary<string, string> translations = null;
            try
            {
                translations = PluginHelper.ReadPluginTranslations(DirectoryConstants.PluginTranslationsFile(pluginType.Name), defaultTranslations);
            } catch (Exception e)
            {
                await logger.LogExceptionAsync(e, $"Failed to load {pluginType.Name} translations!");
                return null;
            }

            IPlugin pluginInstance;

            using (var scope = lifetimeScope.BeginLifetimeScope(cb =>
            {
                if (configuration != null)
                cb.RegisterInstance(configuration).As(configurationType).SingleInstance().ExternallyOwned();
                cb.RegisterInstance(translations).As<IDictionary<string, string>>().SingleInstance().ExternallyOwned();
                cb.RegisterType(pluginType).As(pluginType).As<IPlugin>().SingleInstance().ExternallyOwned();
            }))
            {
                pluginInstance = scope.Resolve(pluginType) as IPlugin;
            }

            // Execute plugin LoadAsync method
            try
            {
                await pluginInstance.LoadAsync();
            } catch (Exception e)
            {
                await logger.LogExceptionAsync(e, $"An exception occurated when executing {pluginType.Name} LoadAsync method");
                await DeactivatePluginAsync(pluginInstance);
                return null;
            }

            activatedPlugins.Add(pluginInstance);
            OnPluginActivated?.Invoke(pluginInstance);
            return pluginInstance;
        }

        public async Task LoadPluginsAsync()
        {
            IEnumerable<FileInfo> pluginFiles = new DirectoryInfo(DirectoryConstants.PluginsDirectory).GetFiles("*.dll", SearchOption.AllDirectories);

            foreach (var file in pluginFiles)
            {
                await LoadPluginAsync(file.FullName);
            }
        }

        public async Task<IPlugin> LoadPluginAsync(string fileFullName)
        {
            var assembly = Assembly.LoadFile(fileFullName);
            loadedPlugins.Add(assembly);
            return await ActivatePluginAsync(assembly);
        }

        public async Task DeactivatePluginAsync(IPlugin pluginInstance)
        {
            // Execute plugin UnloadAsync method
            try
            {
                await pluginInstance.UnloadAsync();
            } catch (Exception e)
            {
                await logger.LogExceptionAsync(e, $"An exception occurated when executing {pluginInstance.Name} UnloadAsync method");
            }

            activatedPlugins.Remove(pluginInstance);
            OnPluginDeactivated?.Invoke(pluginInstance);
        }

        public async Task DeactivatePluginsAsync()
        {
            foreach (var pluginInstance in activatedPlugins)
            {
                await DeactivatePluginAsync(pluginInstance);
            }
        }
    }
}
