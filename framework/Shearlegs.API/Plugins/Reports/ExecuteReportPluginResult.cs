using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.API.Plugins.Reports
{
    public class ExecuteReportPluginResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public IReportFile ReportFile { get; set; }

        public string ErrorResponse => $"<strong>{Message}</strong><br />{Exception}";
    }
}
