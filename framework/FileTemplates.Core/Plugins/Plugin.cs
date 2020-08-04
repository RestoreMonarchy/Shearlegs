using FileTemplates.API.Plugins;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FileTemplates.Core.Plugins
{
    public class Plugin : IPlugin
    {
        public Plugin()
        {
            Assembly = GetType().Assembly;
            Name = GetType().Name;
            Version = Assembly.GetName().Version.ToString();
        }

        public virtual string Name { get; }
        public virtual string Version { get; }
        public Assembly Assembly { get; }

        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task UnloadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
