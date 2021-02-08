using OfficeOpenXml;
using OfficeOpenXml.Table;
using Shearlegs.API.Plugins.Reports;
using Shearlegs.Core.Plugins.Reports;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SimpleQuery
{
    public class SimpleQueryPlugin : ReportPlugin
    {
        public override string Name => "SimpleQuery";
        public override string Version => "1.0.0";

        private readonly SimpleQueryParameters parameters;

        public SimpleQueryPlugin(SimpleQueryParameters parameters)
        {
            this.parameters = parameters;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public override async Task<IReportFile> GenerateReportAsync()
        {
            var report = new ReportFile()
            {
                Name = parameters.OutputFileName,
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
                        
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add(parameters.WorksheetName);

                using (var conn = new SqlConnection(parameters.ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(parameters.Query, conn);

                    var reader = await cmd.ExecuteReaderAsync();

                    await sheet.Cells["A1"].LoadFromDataReaderAsync(reader, true, parameters.WorksheetName, TableStyles.Light10);

                    await reader.CloseAsync();
                }

                report.Data = await package.GetAsByteArrayAsync();
            }

            return report;
        }
    }
}