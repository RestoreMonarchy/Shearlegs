using FileTemplates.API.Plugins.Delegates;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileTemplates.API.Plugins
{
    public interface IPluginManager
    {
        IEnumerable<IPlugin> ActivatedPlugins { get; }
        IEnumerable<Assembly> LoadedPlugins { get; }
        Task<IPlugin> ActivatePluginAsync(Assembly assembly);
        Task LoadPluginsAsync(string directory);
        Task<bool> TryLoadPluginAsync(string directory, string fileName);
        Task UnloadPluginAsync(string name);
        Task UnloadPluginsAsync();

        event PluginLoaded OnPluginLoaded;
    }
}
