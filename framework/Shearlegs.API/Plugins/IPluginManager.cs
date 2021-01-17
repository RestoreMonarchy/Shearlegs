using Shearlegs.API.Plugins.Delegates;
using Shearlegs.API.Reports;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.API.Plugins
{
    public interface IPluginManager
    {
        Task<IReportFile> ExecuteReportPluginAsync(string pluginName, string jsonParameters, byte[] pluginData, IEnumerable<byte[]> libraries);
    }
}
