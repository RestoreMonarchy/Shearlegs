using Shearlegs.API.Plugins.Attributes;

namespace SimpleQuery
{
    [Parameters]
    public class SimpleQueryParameters
    {
        public string OutputFileName { get; set; }
        public string Query { get; set; } = "SELECT * FROM dbo.Reports;";
        public string WorksheetName { get; set; }
        public string ConnectionString { get; set; }
    }
}