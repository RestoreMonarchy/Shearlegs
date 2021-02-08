using System;
using System.Collections.Generic;
using System.Text;

namespace Shearlegs.API.Plugins.Reports
{
    public interface IReportTemplate
    {
        public byte[] Data { get; }
        public string MimeType { get; }
        public string FileName { get; }
    }
}
