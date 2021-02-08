using Shearlegs.API.Plugins;
using Shearlegs.API.Plugins.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Core.Plugins.Reports
{
    public class ReportTemplate : IReportTemplate
    {
        public byte[] Data { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
    }
}
