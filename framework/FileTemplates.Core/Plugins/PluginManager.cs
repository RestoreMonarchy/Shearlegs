using FileTemplates.API.Plugins;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileTemplates.Core.Plugins
{
    public class PluginManager : IPluginManager
    {
        private readonly List<IPlugin> activatedPlugins;

        public PluginManager()
        {
            activatedPlugins = new List<IPlugin>();
        }

        public IEnumerable<IPlugin> ActivatedPlugins => activatedPlugins;

        public async Task ActivatePluginAsync(Assembly assembly)
        {
            assembly.Get
        }
    }
}
