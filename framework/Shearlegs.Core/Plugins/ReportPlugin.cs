using Shearlegs.API.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Core.Plugins
{
    public class ReportPlugin : PluginBase
    {
        public virtual Task<Stream> GenerateFileAsync(IPluginParameters parameters)
        {
            return null;
        }
    }
}
