using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Models
{
    public class ReportBranchModel
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PluginId { get; set; }

        public List<ReportBranchParameterModel> Parameters { get; set; }
        public ReportBranchPluginModel Plugin { get; set; }
        public ReportModel Report { get; set; }
    }
}
