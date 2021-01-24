using Shearlegs.API.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Core.Plugins
{
    public class Template : ITemplate
    {
        public byte[] Data { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }
    }
}
