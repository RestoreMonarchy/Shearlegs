using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using Shearlegs.Core.Reports;
using System.Collections.Generic;
using System.IO;
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

        public override async Task<IReportFile> GenerateReportAsync(IReportParameters parameters)
        {
            var report = new ReportFile()
            {
                Name = "sample.txt",
                MimeType = "text/plain"
            };

            using (var ms = new MemoryStream())
            {
                TextWriter tw = new StreamWriter(ms);
                await tw.WriteAsync("Siemaneczko ziomeczki!");
                await tw.FlushAsync();
                ms.Position = 0;
                report.Data = ms.ToArray();
            }

            return report;
        }
    }
}