using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Models
{
    public class ReportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PluginId { get; set; }
        public bool Enabled { get; set; }

        public List<ReportParameterModel> Parameters { get; set; }
        public ReportPluginModel Plugin { get; set; }
    }
}
