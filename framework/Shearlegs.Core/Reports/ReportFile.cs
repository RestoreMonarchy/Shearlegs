using Shearlegs.API.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.Core.Reports
{
    public class ReportFile : IReportFile
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public byte[] Data { get; set; }
    }
}
