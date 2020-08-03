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
            Name = Assembly.GetName().Name;
            Assembly = GetType().Assembly;
        }

        public virtual string Name { get; }
        public virtual Assembly Assembly { get; }


        public virtual Task<Stream> GenerateFileAsync(JObject data)
        {
            return Task.FromResult(Stream.Null);
        }
    }
}
