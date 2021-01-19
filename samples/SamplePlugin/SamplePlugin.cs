using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using Shearlegs.Core.Reports;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SamplePlugin
{
    public class SampleParameters : Parameters
    {
        public int FileId { get; set; }
        public string Message { get; set; } = "Welcome everybody";
    }

    public class SamplePlugin : ReportPlugin<SampleParameters>
    {
        public override string Name => "SamplePlugin";

        private readonly ILogger logger;

        public SamplePlugin(ILogger logger)
        {
            this.logger = logger;
        }

        public override async Task<IReportFile> GenerateReportAsync()
        {
            var report = new ReportFile()
            {
                Name = $"{Parameters.FileId}.txt",
                MimeType = "text/plain"
            };

            using (var ms = new MemoryStream())
            {
                TextWriter tw = new StreamWriter(ms);
                await tw.WriteAsync(Parameters.Message);
                await tw.FlushAsync();
                ms.Position = 0;
                report.Data = ms.ToArray();
            }

            return report;
        }
    }
}