using Dapper;
using OfficeOpenXml;
using Shearlegs.API.Logging;
using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using Shearlegs.Core.Reports;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace SamplePlugin
{
    [Parameters]
    public class SampleParameters
    {
        public int FileId { get; set; }
        public string Message { get; set; } = "Welcome everybody";
        public byte[] Template { get; set; }
        public string ConnectionString { get; set; }
    }

    public class SamplePlugin : ReportPlugin
    {
        public override string Name => "SamplePlugin";

        private readonly ILogger logger;
        private readonly SampleParameters parameters;
        private readonly ITemplate template;

        public SamplePlugin(ILogger logger, SampleParameters parameters, ITemplate template)
        {
            this.logger = logger;
            this.parameters = parameters;
            this.template = template;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public override async Task<IReportFile> GenerateReportAsync()
        {
            var report = new ReportFile()
            {
                Name = template.FileName,
                MimeType = template.MimeType
            };

            var conn = new SqlConnection(parameters.ConnectionString);
            var reports = await conn.QueryAsync<string>("SELECT Name FROM dbo.Reports;");

            using (var ms = new MemoryStream(parameters.Template))
            {
                using (var package = new ExcelPackage(ms))
                {
                    var shit = package.Workbook.Worksheets["shit"];

                    shit.Cells.LoadFromCollection(reports);

                    report.Data = await package.GetAsByteArrayAsync();
                }
            }

            return report;
        }
    }
}