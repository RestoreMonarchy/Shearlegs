using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Models
{
    public class ReportBranchPluginModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Version { get; set; }
        public string Changelog { get; set; }
        public byte[] Content { get; set; }
        public byte[] TemplateContent { get; set; }
        public string TemplateMimeType { get; set; }
        public string TemplateFileName { get; set; }
        public DateTime CreateDate { get; set; }

        public List<ReportBranchPluginLibraryModel> Libraries { get; set; }
    }
}
