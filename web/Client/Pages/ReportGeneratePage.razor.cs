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
    public partial class ReportGeneratePage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Parameter]
        public int ReportId { get; set; }

        public ReportModel Report { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine(ReportId);
            Report = await HttpClient.GetFromJsonAsync<ReportModel>("api/reports/" + ReportId);
        }

        private string json;
        private ReportArchiveModel reportArchive;

        public async Task GenerateReportAsync()
        {
            var response = await HttpClient.PostAsync($"api/reports/{Report.Id}/execute", new StringContent(json));
            reportArchive = await response.Content.ReadFromJsonAsync<ReportArchiveModel>();
        }
    }
}
