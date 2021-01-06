using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Core.Reports
{
    public class ReportPlugin : PluginBase
    {
        public virtual Task<IReportFile> GenerateReportAsync(IReportParameters parameters)
        {
            return null;
        }
    }
}
