using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Core.Reports
{
    public class ReportPlugin<T> : PluginBase, IReportPlugin<T> where T : class
    {
        public T Parameters { get; set; }

        public virtual Task<IReportFile> GenerateReportAsync()
        {
            return null;
        }
    }
}
