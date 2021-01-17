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

        [HttpGet]
        public async Task<IActionResult> GetPluginsAsync()
        {
            return Ok(await reportsRepository.GetReportsAsync());
        }

        [HttpPost("plugin")]
        public async Task<IActionResult> PostPluginAsync([FromBody] ReportPluginModel reportPluginModel)
        {
            await reportsRepository.AddReportPluginAsync(reportPluginModel);
            return Ok(reportPluginModel);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ReportModel reportModel)
        {
            await reportsRepository.AddReportAsync(reportModel);
            return Ok(reportModel);
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
            var reportModel = await reportsRepository.GetReportAsync(reportId);
            
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            var report = await pluginManager.ExecuteReportPluginAsync(reportModel.Name, requestBody, reportModel.Plugin.Content, Array.Empty<byte[]>());

            var reportArchive = new ReportArchiveModel()
            {
                Name = report.Name,
                Content = report.Data,
                MimeType = report.MimeType,
                PluginName = report.Name,
                Parameters = requestBody
            };

            reportArchive = await reportsRepository.ArchiveReportAsync(reportArchive);
            return Ok(reportArchive);
        }
    }
}
