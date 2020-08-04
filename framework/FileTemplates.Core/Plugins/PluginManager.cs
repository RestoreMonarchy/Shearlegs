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

        public PluginManager(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
            loadedPlugins = new List<Assembly>();
            activatedPlugins = new List<IPlugin>();
        }

        public event PluginLoaded OnPluginLoaded;

        public IEnumerable<Assembly> LoadedPlugins => loadedPlugins;
        public IEnumerable<IPlugin> ActivatedPlugins => activatedPlugins;        

        public Task<IPlugin> ActivatePluginAsync(Assembly assembly)
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

            activatedPlugins.Add(pluginInstance);
            return Task.FromResult(pluginInstance);
        }

        public async Task LoadPluginsAsync(string directory)
        {
            IEnumerable<FileInfo> pluginFiles = new DirectoryInfo(directory).GetFiles("*.dll", SearchOption.AllDirectories);

            foreach (var file in pluginFiles)
            {
                await LoadPluginAsync(file.FullName);
            }
        }

        private async Task LoadPluginAsync(string fileFullName)
        {
            var assembly = Assembly.LoadFile(fileFullName);
            loadedPlugins.Add(assembly);
            OnPluginLoaded?.Invoke(await ActivatePluginAsync(assembly));
        }

        public async Task<bool> TryLoadPluginAsync(string directory, string fileName)
        {
            FileInfo file = new DirectoryInfo(directory).GetFiles($"{fileName}.dll", SearchOption.AllDirectories).FirstOrDefault();

            if (file != null)
            {
                await LoadPluginAsync(file.FullName);
                return true;
            }
            return false;
        }

        public Task UnloadPluginAsync(string name)
        {
            var plugin = activatedPlugins.FirstOrDefault(x => x.Name.Equals(name));
            if (plugin != null)
            {
                activatedPlugins.Remove(plugin);
                loadedPlugins.Remove(plugin.Assembly);
            }
            return Task.CompletedTask;
        }

        public Task UnloadPluginsAsync()
        {
            activatedPlugins.Clear();
            loadedPlugins.Clear();
            return Task.CompletedTask;
        }
    }
}
