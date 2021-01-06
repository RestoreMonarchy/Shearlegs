using Microsoft.Extensions.DependencyInjection;
using Shearlegs.API;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Delegates;
using Shearlegs.Core.Constants;
using Shearlegs.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Shearlegs.Core.Reports
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

        public async Task<IPlugin> ActivatePluginAsync(Assembly assembly)
        {            
            var pluginType = assembly.GetTypes().FirstOrDefault(x => x.GetInterface(nameof(IPlugin)) != null);

            if (pluginType == null)
            {
                await logger.LogInformationAsync($"{assembly.GetName().Name} is not valid plugin assembly!");
                return null;
            }

            var services = assembly.GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);

            // Add plugin as singleton service
            IPlugin pluginInstance;
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(pluginType);

            // Add logger service with ISession
            serviceCollection.AddSingleton(session);
            serviceCollection.AddTransient<ILogger, Logger>();            
            
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
                await pluginInstance.ActivateAsync();
            } catch (Exception e)
            {
                await logger.LogExceptionAsync(e, $"An exception occurated when loading {pluginInstance.Name} {pluginInstance.Version}");
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
                try
                {
                    await LoadPluginAsync(file.FullName);
                } catch (Exception e)
                {
                    await logger.LogExceptionAsync(e, $"An exception occurated when trying to load plugin {file.FullName}");
                }                
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
                await pluginInstance.DeactivateAsync();
            } catch (Exception e)
            {
                await logger.LogExceptionAsync(e, $"An exception occurated when unloading {pluginInstance.Name} {pluginInstance.Version}");
            }

            activatedPlugins.Remove(pluginInstance);
            OnPluginDeactivated?.Invoke(pluginInstance);
        }

        public async Task DeactivatePluginsAsync()
        {
            while (activatedPlugins.Count != 0)
            {
                await DeactivatePluginAsync(activatedPlugins[0]);
            }
        }
    }
}