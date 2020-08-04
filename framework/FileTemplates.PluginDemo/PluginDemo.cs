using FileTemplates.API.Logging;
using FileTemplates.Core.Plugins;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace FileTemplates.PluginDemo
{
    public class PluginDemo : Plugin
    {
        private readonly ILogger logger;
        public PluginDemo(ILogger logger)
        {
            this.logger = logger;
            logger.LogAsync("Hello people");
        }

        public override string Name => "PluginDemo";
        public override async Task<Stream> GenerateFileAsync(JObject data)
        {
            await logger.LogAsync("PluginDemo generating file");
            return await base.GenerateFileAsync(data);
        }
    }
}
