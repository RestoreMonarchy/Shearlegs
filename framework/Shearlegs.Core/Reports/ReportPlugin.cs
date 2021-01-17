using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Core.Reports
{
    public class ReportPlugin : PluginBase, IReportPlugin
    {
        public virtual Task<IReportFile> GenerateReportAsync()
        {
            return null;
        }
    }

    public class ReportPlugin<T> : ReportPlugin where T : IReportParameters
    {
        public T Parameters { get; }
    }
}
