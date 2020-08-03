using FileTemplates.API.Plugins;
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
        private readonly List<Assembly> loadedPlugins;
        private readonly List<IPlugin> activatedPlugins;

        public PluginManager()
        {
            loadedPlugins = new List<Assembly>();
            activatedPlugins = new List<IPlugin>();
        }

        public IEnumerable<Assembly> LoadedPlugins => loadedPlugins;
        public IEnumerable<IPlugin> ActivatedPlugins => activatedPlugins;        

        public async Task ActivatePluginAsync(Assembly assembly)
        {
            var pluginType = assembly.GetTypes().FirstOrDefault(x => x.IsAssignableFrom(typeof(IPlugin)));
            activatedPlugins.Add(Activator.CreateInstance(pluginType) as IPlugin);
        }

        public async Task LoadPluginsAsync(string directory)
        {
            IEnumerable<FileInfo> pluginFiles = new DirectoryInfo(directory).GetFiles(".dll", SearchOption.AllDirectories);

            foreach (var file in pluginFiles)
            {
                var assembly = Assembly.LoadFile(file.FullName);
                loadedPlugins.Add(assembly);
                await ActivatePluginAsync(assembly);
            }
        }

        public async Task UnloadPluginAsync(string name)
        {
            var plugin = activatedPlugins.FirstOrDefault(x => x.Name.Equals(name));
            if (plugin != null)
            {
                activatedPlugins.Remove(plugin);
                loadedPlugins.Remove(plugin.Assembly);
            }
        }

        public async Task UnloadPluginsAsync()
        {
            activatedPlugins.Clear();
            loadedPlugins.Clear();
        }
    }
}
