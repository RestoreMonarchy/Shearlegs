using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FileTemplates.API.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        Assembly Assembly { get; }
        Task<Stream> GenerateFileAsync(JObject data);
    }
}
