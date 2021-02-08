using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.API.Plugins.Reports
{
    public class ExecuteReportPluginArguments
    {
        public string PluginName { get; set; }
        public string JsonParameters { get; set; }
        public byte[] PluginData { get; set; }
        public IReportTemplate ReportTemplate { get; set; }
        public IEnumerable<byte[]> LibrariesData { get; set; }
    }
}
