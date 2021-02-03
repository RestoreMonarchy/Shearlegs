using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shearlegs.Web.Client.Extensions;
using Shearlegs.Web.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Shearlegs.Web.Client.Pages
{
    [Authorize]
    public partial class ReportGeneratePage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public int ReportId { get; set; }

        public ReportModel Report { get; set; }
        public ReportBranchModel Branch { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine(ReportId);
            Report = await HttpClient.GetFromJsonAsync<ReportModel>("api/reports/" + ReportId);
            await ReloadBranchAsync(Report.Branches.FirstOrDefault()?.Id ?? 0);
        }

        private async Task OnChangeBranchAsync(ChangeEventArgs e)
        {
            await ReloadBranchAsync((int)e.Value);
        }

        private async Task ReloadBranchAsync(int branchId)
        {
            if (branchId == 0)
            {
                Branch = null;
                return;
            }

            Branch = await HttpClient.GetFromJsonAsync<ReportBranchModel>("api/reports/branches/" + branchId);
        }

        private ReportArchiveModel reportArchive;

        public async Task GenerateReportAsync()
        {
            string json = await JsRuntime.GetFormDataJsonAsync("reportParameters");
            Console.WriteLine(json);
            var response = await HttpClient.PostAsync($"api/reports/{Branch.Id}/execute", new StringContent(json));
            reportArchive = await response.Content.ReadFromJsonAsync<ReportArchiveModel>();
        }
    }
}
