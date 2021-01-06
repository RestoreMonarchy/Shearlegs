using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.Core.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamplePlugin
{
    public class SamplePlugin : PluginBase
    {
        public override string Name => "SamplePlugin";

        private readonly ILogger logger;
        private readonly SamplePluginConfiguration configuration;
        private readonly IDictionary<string, string> translations;

        public SamplePlugin(ILogger logger, SamplePluginConfiguration configuration, IDictionary<string, string> translations)
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

        public override async Task LoadAsync()
        {
            await logger.LogAsync($"Hello {translations["Hello"]}");
            await logger.LogAsync($"{configuration.HelloWorld}");
            await logger.LogAsync($"Hello folks from {Name}!");
        }

        public override async Task UnloadAsync()
        {
            await logger.LogAsync($"Goodbye folks from {Name}!");
        }
    }
}