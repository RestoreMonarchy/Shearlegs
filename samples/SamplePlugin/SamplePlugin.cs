using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using Shearlegs.Core.Reports;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SamplePlugin
{
    [Parameters]
    public class SampleParameters
    {
        public int FileId { get; set; }
        public string Message { get; set; } = "Welcome everybody";
    }

    public class SamplePlugin : ReportPlugin
    {
        public override string Name => "SamplePlugin";

        private readonly ILogger logger;
        private readonly SampleParameters parameters;

        public SamplePlugin(ILogger logger, SampleParameters parameters)
        {
            this.logger = logger;
            this.parameters = parameters;
        }

        public override async Task<IReportFile> GenerateReportAsync()
        {
            var report = new ReportFile()
            {
                Name = $"{parameters.FileId}.txt",
                MimeType = "text/plain"
            };

            using (var ms = new MemoryStream())
            {
                TextWriter tw = new StreamWriter(ms);
                await tw.WriteAsync(parameters.Message);
                await tw.FlushAsync();
                ms.Position = 0;
                report.Data = ms.ToArray();
            }

            return report;
        }
    }
}