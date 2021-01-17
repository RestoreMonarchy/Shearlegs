using Microsoft.AspNetCore.Mvc;
using Shearlegs.API.Plugins;
using Shearlegs.API.Reports;
using Shearlegs.Core.Reports;
using Shearlegs.Web.Server.Repositories;
using Shearlegs.Web.Shared.Models;
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
        private readonly ReportsRepository reportsRepository;

        public ReportsController(IPluginManager pluginManager, ReportsRepository reportsRepository)
        {
            this.pluginManager = pluginManager;
            this.reportsRepository = reportsRepository;
        }

        [HttpGet("plugins")]
        public IActionResult GetPluginsAsync()
        {
            return Ok(pluginManager.ActivatedPlugins.Select(x => x.Name));
        }


        [HttpGet("archive/{id}/file")]
        public async Task<IActionResult> GetReportArchiveAsync(int id)
        {
            var reportArchive = await reportsRepository.GetReportArchiveAsync(id);
            return File(reportArchive.Content, reportArchive.MimeType, reportArchive.Name);
        }

        [HttpPost("{reportId}/execute")]
        public async Task<IActionResult> PostAsync(int reportId)
        {
            var report = await reportsRepository.GetReportAsync(reportId);
            await pluginManager.ExecuteReportPluginAsync(report.Name, report.Plugin.Content, Array.Empty<byte[]>());

            ReportPlugin plugin = pluginManager.ActivatedPlugins.Where(x => x as ReportPlugin != null)
                .FirstOrDefault(x => x.Name == pluginName) as ReportPlugin;
            
            if (plugin == null)
            {
                return BadRequest();
            } else
            {
                string requestBody;
                using (var reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                IReportParameters parameters = new ReportParameters(requestBody);
                var report = await plugin.GenerateReportAsync(parameters);

                var reportArchive = new ReportArchiveModel() 
                { 
                    Name = report.Name,
                    Content = report.Data,
                    MimeType = report.MimeType,
                    PluginName = plugin.Name,
                    Parameters = requestBody
                };

                reportArchive = await reportsRepository.ArchiveReportAsync(reportArchive);
                return Ok(reportArchive);
            }
        }
    }
}
