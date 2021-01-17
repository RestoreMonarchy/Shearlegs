using Shearlegs.API.Plugins;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.API.Reports
{
    public interface IReportPlugin<T> : IPlugin where T : class
    {
        T Parameters { get; }

        Task<IReportFile> GenerateReportAsync();        
    }
}
