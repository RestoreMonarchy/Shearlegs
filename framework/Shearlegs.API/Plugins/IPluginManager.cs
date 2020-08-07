using Shearlegs.API.Plugins.Delegates;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.API.Plugins
{
    public interface IPluginManager
    {
        IEnumerable<IPlugin> ActivatedPlugins { get; }
        IEnumerable<Assembly> LoadedPlugins { get; }

        Task<IPlugin> ActivatePluginAsync(Assembly assembly);
        Task LoadPluginsAsync();
        Task<IPlugin> LoadPluginAsync(string fileFullName);
        Task DeactivatePluginAsync(IPlugin pluginInstance);
        Task DeactivatePluginsAsync();

        event PluginActivated OnPluginActivated;
        event PluginDeactivated OnPluginDeactivated;
    }
}
