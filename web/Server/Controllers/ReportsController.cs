using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Shearlegs.API.Plugins.Reports;
using Shearlegs.Core.Plugins.Reports;
using Shearlegs.Web.Server.Repositories;
using Shearlegs.Web.Shared.Constants;
using Shearlegs.Web.Shared.Models;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Shearlegs.Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportPluginManager reportPluginManager;
        private readonly ReportsRepository reportsRepository;

        public ReportsController(IReportPluginManager reportPluginManager, ReportsRepository reportsRepository)
        {
            this.reportPluginManager = reportPluginManager;
            this.reportsRepository = reportsRepository;
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost("users")]
        public async Task<IActionResult> PostReportUserAsync([FromBody] ReportUserModel reportUser)
        {
            reportUser.AdminId = int.Parse(User.Identity.Name);
            return Ok(await reportsRepository.AddReportUserAsync(reportUser));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpDelete("users/{reportUserId}")]
        public async Task<IActionResult> DeleteReportUserAsync(int reportUserId)
        {
            await reportsRepository.DeleteReportUserAsync(reportUserId);
            return Ok();
        }


        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost("secrets")]
        public async Task<IActionResult> PostSecretAsync([FromBody] ReportBranchSecretModel secret)
        {
            return Ok(await reportsRepository.AddReportBranchSecretAsync(secret));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpDelete("secrets/{secretId}")]
        public async Task<IActionResult> DeleteSecretAsync(int secretId)
        {
            await reportsRepository.RemoveReportBranchSecretAsync(secretId);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost("branches")]
        public async Task<IActionResult> PostBranchAsync([FromBody] ReportBranchModel branch)
        {
            return Ok(await reportsRepository.AddReportBranchAsync(branch));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPut("branches")]
        public async Task<IActionResult> PutBranchAsync([FromBody] ReportBranchModel branch)
        {
            await reportsRepository.UpdateReportBranchAsync(branch);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetReportsAsync()
        {
            int userId = User.IsInRole(RoleConstants.AdminRoleId) ? 0 : int.Parse(User.Identity.Name);
            return Ok(await reportsRepository.GetReportsAsync(userId));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ReportModel reportModel)
        {
            await reportsRepository.AddReportAsync(reportModel);
            return Ok(reportModel);
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPut]
        public async Task<IActionResult> PutReportAsync([FromBody] ReportModel reportModel)
        {
            await reportsRepository.UpdateReportAsync(reportModel);
            return Ok();
        }

        [Authorize]
        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetReportAsync(int reportId)
        {
            if (!User.IsInRole(RoleConstants.AdminRoleId)
                && !await reportsRepository.HasPermissionAsync(int.Parse(User.Identity.Name), reportId))
            {
                return Unauthorized();
            }

            return Ok(await reportsRepository.GetReportAsync(reportId));
        }

        [Authorize]
        [HttpGet("branches/{branchId}")]
        public async Task<IActionResult> GetReportBranchAsync(int branchId)
        {
            var branch = await reportsRepository.GetReportBranchAsync(branchId);
            if (!User.IsInRole(RoleConstants.AdminRoleId)
                && !await reportsRepository.HasPermissionAsync(int.Parse(User.Identity.Name), branch.ReportId))
            {
                return Unauthorized();
            }

            return Ok(branch);
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost("parameters")]
        public async Task<IActionResult> PostReportParameterAsync([FromBody] ReportBranchParameterModel reportParameter)
        {
            return Ok(await reportsRepository.AddReportBranchParameterAsync(reportParameter));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpDelete("parameters/{reportParameterId}")]
        public async Task<IActionResult> DeleteReportParameterAsync(int reportParameterId)
        {
            await reportsRepository.RemoveReportBranchParameterAsync(reportParameterId);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost("plugin")]
        public async Task<IActionResult> PostPluginAsync([FromBody] ReportBranchPluginModel reportPluginModel)
        {
            await reportsRepository.AddReportBranchPluginAsync(reportPluginModel);
            return Ok(reportPluginModel);
        }

        [Authorize]
        [HttpGet("archive/{id}/file")]
        public async Task<IActionResult> GetReportArchiveAsync(int id)
        {
            var reportArchive = await reportsRepository.GetReportArchiveAsync(id);

            return File(reportArchive.Content, reportArchive.MimeType, reportArchive.Name);
        }

        [Authorize]
        [HttpPost("{branchId}/execute")]
        public async Task<IActionResult> PostAsync(int branchId)
        {
            var branchModel = await reportsRepository.GetReportPluginAsync(branchId);

            if (!User.IsInRole(RoleConstants.AdminRoleId) 
                && !await reportsRepository.HasPermissionAsync(int.Parse(User.Identity.Name), branchModel.ReportId))
            {
                return Unauthorized();
            }
            
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            var jObject = JObject.Parse(requestBody);

            foreach (var secret in branchModel.Secrets)
            {
                jObject.Add(secret.Name, JToken.FromObject(secret.Value));
            }

            requestBody = jObject.ToString();

            IReportTemplate template = null;

            if (branchModel.Plugin.TemplateContent != null)
            {
                template = new ReportTemplate()
                {
                    Data = branchModel.Plugin.TemplateContent,
                    MimeType = branchModel.Plugin.TemplateMimeType,
                    FileName = branchModel.Plugin.TemplateFileName
                };
            }

            var args = new ExecuteReportPluginArguments()
            {
                PluginName = branchModel.Report.Name,
                JsonParameters = requestBody,
                PluginData = branchModel.Plugin.Content,
                ReportTemplate = template,
                LibrariesData = branchModel.Plugin.Libraries.Select(x => x.Content)
            };

            var result = await reportPluginManager.ExecuteReportPluginAsync(args);

            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, result.ErrorResponse);
            }

            var reportArchive = new ReportArchiveModel()
            {
                Name = result.ReportFile.Name,
                Content = result.ReportFile.Data,
                MimeType = result.ReportFile.MimeType,
                PluginId = branchModel.Plugin.Id,
                Parameters = requestBody
            };

            reportArchive = await reportsRepository.ArchiveReportAsync(reportArchive);
            return Ok(reportArchive);
        }
    }
}
