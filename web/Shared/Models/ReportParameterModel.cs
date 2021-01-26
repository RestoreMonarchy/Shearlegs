using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Models
{
    public class ReportParameterModel
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string InputName { get; set; }
        public bool IsMandatory { get; set; }
    }
}
