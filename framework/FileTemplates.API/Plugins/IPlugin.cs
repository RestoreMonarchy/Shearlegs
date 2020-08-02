using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace FileTemplates.API.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        Task<Stream> GenerateFileAsync(JObject data);
    }
}
