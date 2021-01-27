using Microsoft.AspNetCore.Components;
using Shearlegs.Web.Client.Shared.Components;
using Shearlegs.Web.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Shearlegs.Web.Client.Pages.Admin
{
    public partial class ReportsAdminPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        public Modal<ReportModel> Modal { get; set; }

        public List<ReportModel> Reports { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Reports = await HttpClient.GetFromJsonAsync<List<ReportModel>>("api/reports");
        }

        private async Task PostReportAsync(ReportModel report)
        {
            var response = await HttpClient.PostAsJsonAsync("api/reports", report);
            Reports.Add(await response.Content.ReadFromJsonAsync<ReportModel>());
        }

        private async Task PutReportAsync(ReportModel report)
        {
            await HttpClient.PutAsJsonAsync("api/reports", report);
        }

        public ReportParameterModel ReportParameter { get; set; } = new ReportParameterModel();

        public async Task AddParameterAsync()
        {
            var response = await HttpClient.PostAsJsonAsync("api/reports/parameters", ReportParameter);
            ReportParameter = await response.Content.ReadFromJsonAsync<ReportParameterModel>();
            Modal.Model.Parameters.Add(ReportParameter);
            ReportParameter = new ReportParameterModel() { InputType = "text" };
        }

        public async Task RemoveParameterAsync(ReportParameterModel reportParameter)
        {
            await HttpClient.DeleteAsync("api/reports/parameters/" + reportParameter.Id);
            Modal.Model.Parameters.Remove(reportParameter);
        }

        private async Task EditReportAsync(ReportModel report)
        {
            ReportParameter = new ReportParameterModel()
            {
                ReportId = report.Id,
                InputType = "text"
            };
            await Modal.UpdateAsync(report);
        } 
        private async Task AddReportAsync() => await Modal.CreateAsync();
    }
}
