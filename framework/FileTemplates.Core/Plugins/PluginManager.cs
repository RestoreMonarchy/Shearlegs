using Autofac;
using FileTemplates.API.Plugins;
using FileTemplates.API.Plugins.Delegates;
using FileTemplates.Core.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FileTemplates.Core.Plugins
{
    public class PluginManager : IPluginManager
    {
        private readonly ILifetimeScope lifetimeScope;

        private readonly List<Assembly> loadedPlugins;
        private readonly List<IPlugin> activatedPlugins;

        public event PluginActivated OnPluginActivated;
        public event PluginDeactivated OnPluginDeactivated;

        public IEnumerable<Assembly> LoadedPlugins => loadedPlugins;
        public IEnumerable<IPlugin> ActivatedPlugins => activatedPlugins;

        public PluginManager(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
            loadedPlugins = new List<Assembly>();
            activatedPlugins = new List<IPlugin>();
        }

        public async Task<IPlugin> ActivatePluginAsync(Assembly assembly)
        {            
            var pluginType = assembly.GetTypes().FirstOrDefault(x => x.GetInterface(nameof(IPlugin)) != null);
            var configurationType = assembly.GetTypes().FirstOrDefault(x => x.GetCustomAttribute<ConfigurationAttribute>()?.PluginType?.Equals(pluginType) ?? false);
            var defaultTranslations = pluginType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(x => x.GetCustomAttribute<DefaultTranslationsAttribute>() != null)?
                .GetValue(null) as IDictionary<string, string> ?? null;

            Directory.CreateDirectory(DirectoryConstants.PluginDirectory(pluginType.Name));

            IPlugin pluginInstance;

            // TODO: add try-catch and log exception when reading plugin configuration
            var configuration = typeof(PluginHelper)
                .GetMethod(nameof(PluginHelper.ReadPluginConfiguration), BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(configurationType)
                .Invoke(null, new object[] { DirectoryConstants.PluginConfigurationFile(pluginType.Name) });

            IDictionary<string, string> translations = null;
            if (defaultTranslations != null)
                translations = PluginHelper.ReadPluginTranslations(DirectoryConstants.PluginTranslationsFile(pluginType.Name), defaultTranslations);

            using (var scope = lifetimeScope.BeginLifetimeScope(cb =>
            {
                cb.RegisterInstance(configuration).As(configurationType).SingleInstance().ExternallyOwned();
                if (translations != null)
                    cb.RegisterInstance(translations).As<IDictionary<string, string>>().SingleInstance().ExternallyOwned();
                cb.RegisterType(pluginType).As(pluginType).As<IPlugin>().SingleInstance().ExternallyOwned();
            }))
            {
                pluginInstance = scope.Resolve(pluginType) as IPlugin;
            }

            // TODO: add try-catch and log exception when calling plugin LoadAsync
            await pluginInstance.LoadAsync();

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
            // TODO: add try-catch and log exception when calling plugin UnloadAsync
            await pluginInstance.UnloadAsync();

            activatedPlugins.Remove(pluginInstance);
            loadedPlugins.Remove(pluginInstance.Assembly);

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
