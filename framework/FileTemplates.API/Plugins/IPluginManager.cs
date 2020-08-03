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
        Task ActivatePluginAsync(Assembly assembly);

        Task LoadPluginsAsync(string directory);
        Task UnloadPluginAsync(string name);
        Task UnloadPluginsAsync();
    }
}
