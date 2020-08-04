using Autofac;
using FileTemplates.API.Plugins;
using FileTemplates.API.Plugins.Delegates;
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

            IPlugin pluginInstance;

            using (var scope = lifetimeScope.BeginLifetimeScope(cb =>
            {
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

        public async Task LoadPluginsAsync(string directory)
        {
            IEnumerable<FileInfo> pluginFiles = new DirectoryInfo(directory).GetFiles("*.dll", SearchOption.AllDirectories);

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
