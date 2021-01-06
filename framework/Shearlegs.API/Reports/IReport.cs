using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shearlegs.API.Reports
{
    public interface IReportFile
    {
        public string Name { get; }
        public string MimeType { get; }
        public byte[] Data { get; }
    }
}
