using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Models
{
    public class ReportUserModel
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int UserId { get; set; }
        public int AdminId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
