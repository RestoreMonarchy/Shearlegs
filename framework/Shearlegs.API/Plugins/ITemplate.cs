using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.API.Plugins
{
    public interface ITemplate
    {
        public byte[] Data { get; }
        public string MimeType { get; }
        public string FileName { get; }
    }
}
