using Microsoft.Extensions.DependencyInjection;
using Shearlegs.API;
using Shearlegs.API.Attributes;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Delegates;
using Shearlegs.Core.Constants;
using Shearlegs.Core.Logging;
using Shearlegs.Core.Translations;
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
        private readonly ILogger logger;
        private readonly ISession session;

        private readonly List<Assembly> loadedPlugins;
        private readonly List<IPlugin> activatedPlugins;

        public event PluginActivated OnPluginActivated;
        public event PluginDeactivated OnPluginDeactivated;

        public IEnumerable<Assembly> LoadedPlugins => loadedPlugins;
        public IEnumerable<IPlugin> ActivatedPlugins => activatedPlugins;

        public PluginManager(ILogger logger, ISession session)
        {
            this.logger = logger;
            this.session = session;
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

            var services = assembly.GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);

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


            // Add plugin required services
            IPlugin pluginInstance;

            IServiceCollection serviceCollection = new ServiceCollection();

            if (configuration != null)
                serviceCollection.AddSingleton(configurationType, configuration);

            serviceCollection.AddSingleton(pluginType);
            serviceCollection.AddTransient<ILogger, Logger>();
            serviceCollection.AddSingleton(session);
            
            // Add plugin custom services
            foreach (var service in services)
            {
                serviceCollection.Add(new ServiceDescriptor(service.GetType(), service.GetType(),
                    service.GetCustomAttribute<ServiceAttribute>().Lifetime));
            }

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            pluginInstance = serviceProvider.GetRequiredService(pluginType) as IPlugin;

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
