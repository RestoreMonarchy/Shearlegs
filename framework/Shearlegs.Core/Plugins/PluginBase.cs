using Shearlegs.API.Plugins;
using System.Reflection;

namespace Shearlegs.Core.Reports
{
    public abstract class PluginBase : IPlugin
    {
        public PluginBase()
        {
            Assembly = GetType().Assembly;
            Name = GetType().Name;
            Version = Assembly.GetName().Version.ToString();
            Author = null;
        }

        public virtual string Name { get; }
        public virtual string Version { get; }
        public virtual string Author { get; }

        public Assembly Assembly { get; }
    }
}
