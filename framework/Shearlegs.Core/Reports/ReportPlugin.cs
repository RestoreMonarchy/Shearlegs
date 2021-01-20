using Newtonsoft.Json;
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
            throw new NotImplementedException();
        }
    }
}
