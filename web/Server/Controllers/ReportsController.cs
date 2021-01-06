using Microsoft.AspNetCore.Mvc;
using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using Shearlegs.Core.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shearlegs.Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IPluginManager pluginManager;

        public ReportsController(IPluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
        }

        [HttpPost("{pluginName}")]
        public async Task<IActionResult> PostAsync(string pluginName)
        {
            ReportPlugin plugin = pluginManager.ActivatedPlugins.Where(x => x as ReportPlugin != null)
                .FirstOrDefault(x => x.Name == pluginName) as ReportPlugin;
            
            if (plugin == null)
            {
                return BadRequest();
            } else
            {
                IReportParameters parameters;
                using (var reader = new StreamReader(Request.Body))
                {
                    parameters = new ReportParameters(await reader.ReadToEndAsync());
                }
                var report = await plugin.GenerateReportAsync(parameters);
                return File(report.Data, report.MimeType, report.Name);
            }
        }
    }
}
