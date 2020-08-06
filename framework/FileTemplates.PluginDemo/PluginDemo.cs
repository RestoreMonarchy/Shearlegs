using FileTemplates.API.Logging;
using FileTemplates.API.Plugins;
using FileTemplates.Core.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileTemplates.PluginDemo
{
    public class PluginDemo : Plugin
    {
        private readonly ILogger logger;
        private readonly PluginDemoConfiguration configuration;
        private readonly IDictionary<string, string> translations;

        public PluginDemo(ILogger logger, PluginDemoConfiguration configuration, IDictionary<string, string> translations)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.translations = translations;
        }

        [DefaultTranslations]
        private static IDictionary<string, string> DefaultTranslations = new Dictionary<string, string>()
        {
            { "Hello", "World!" }
        };


        public override string Name => "PluginDemo";

        public override async Task LoadAsync()
        {
            await logger.LogAsync($"Hello {translations["Hello"]}");
            await logger.LogAsync($"{configuration.SampleConfigProperty}");
            await logger.LogAsync($"Hello folks from PluginDemo!");
        }

        public override async Task UnloadAsync()
        {
            await logger.LogAsync($"Goodbye folks from PluginDemo!");
        }
    }
}