using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.Core.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamplePlugin
{
    public class SamplePlugin : ReportPlugin
    {
        public override string Name => "SamplePlugin";

        private readonly ILogger logger;

        public SamplePlugin(ILogger logger)
        {
            this.logger = logger;
        }

        public override async Task ActivateAsync()
        {
            await logger.LogAsync("Siema");
        }

        public override async Task DeactivateAsync()
        {
            await logger.LogAsync("Bajo");
        }
    }
}