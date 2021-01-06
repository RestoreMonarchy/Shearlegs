using Microsoft.AspNetCore.Mvc;
using Shearlegs.API.Plugins;
using Shearlegs.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var plugin = pluginManager.ActivatedPlugins.Where(x => x as ReportPlugin != null).FirstOrDefault(x => x.Name == pluginName) as ReportPlugin;

            if (plugin == null)
            {
                return BadRequest();
            } else
            {
                var report = await plugin.GenerateReportAsync(null);
                return File(report.Data, report.MimeType, report.Name);
            }
        }
    }
}
