using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Models
{
    public class ReportBranchPluginLibraryModel
    {
        public int Id { get; set; }
        public int PluginId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
