using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Models
{
    public class ReportArchiveModel
    {
        public int Id { get; set; }
        public int PluginId { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
        public string Parameters { get; set; }
        public DateTime CreateDate { get; set; }

        public ReportBranchPluginModel Plugin { get; set; }
        public ReportModel Report { get; set; }
    }
}
