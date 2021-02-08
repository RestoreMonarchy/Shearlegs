using Shearlegs.API.Plugins.Reports;
using Shearlegs.Core.Reports;
using System;
using System.Threading.Tasks;

namespace Shearlegs.Core.Plugins.Reports
{
    public class ReportPlugin : PluginBase, IReportPlugin
    {
        public virtual Task<IReportFile> GenerateReportAsync()
        {
            throw new NotImplementedException();
        }
    }
}
