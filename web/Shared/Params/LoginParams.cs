using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Shared.Params
{
    public class LoginParams
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
