using Shearlegs.API.Plugins.Reports;

namespace Shearlegs.Core.Plugins.Reports
{
    public class ReportFile : IReportFile
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public byte[] Data { get; set; }
    }
}
