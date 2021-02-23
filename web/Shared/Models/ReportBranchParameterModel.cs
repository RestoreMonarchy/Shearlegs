using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Models
{
    public class ReportBranchParameterModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        public string Name { get; set; }
        public string InputType { get; set; }
        public bool IsMandatory { get; set; }
    }
}
