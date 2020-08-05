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
        private readonly PluginDemoConfiguration configuration;

        public PluginDemo(ILogger logger, PluginDemoConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public override string Name => "PluginDemo";

        public override async Task LoadAsync()
        {
            await logger.LogAsync($"{configuration.SampleConfigProperty}");
            await logger.LogAsync($"Hello folks from PluginDemo!");
        }

        public override async Task UnloadAsync()
        {
            await logger.LogAsync($"Goodbye folks from PluginDemo!");
        }
    }
}