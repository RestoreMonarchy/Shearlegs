using Shearlegs.API.Plugins;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.API.Plugins.Reports
{
    public interface IReportPlugin : IPlugin
    {
        Task<IReportFile> GenerateReportAsync();
    }
}
