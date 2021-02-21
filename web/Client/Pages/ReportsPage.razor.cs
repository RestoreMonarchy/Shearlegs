using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
    public partial class ReportsPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public List<ReportModel> Reports { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Reports = await HttpClient.GetFromJsonAsync<List<ReportModel>>("api/reports");
        }

        private void GoToGenerateReport(ReportModel report) => NavigationManager.NavigateTo($"/reports/{report.Id}/generate");
    }
}
