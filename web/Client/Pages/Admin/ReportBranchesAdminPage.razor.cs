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
    public partial class ReportBranchesAdminPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Parameter]
        public int ReportId { get; set; }

        public ReportModel Report { get; set; }
        public ReportBranchModel Branch { get; set; }

        public Modal<ReportBranchModel> Modal { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Report = await HttpClient.GetFromJsonAsync<ReportModel>("api/reports/" + ReportId);
            await ReloadBranchAsync(Report.Branches.FirstOrDefault()?.Id ?? 0);
        }

        

        private async Task OnChangeBranchAsync(ChangeEventArgs e)
        {
            await ReloadBranchAsync(int.Parse(e.Value.ToString()));
        }

        private async Task ReloadBranchAsync(int branchId)
        {            
            if (branchId == 0)
            {
                Branch = null;
                return;
            }

            Branch = await HttpClient.GetFromJsonAsync<ReportBranchModel>("api/reports/branches/" + branchId);
            Parameter = new ReportBranchParameterModel()
            {
                BranchId = Branch.Id,
                InputType = "text"
            };
            Secret = new ReportBranchSecretModel()
            {
                BranchId = Branch.Id,
            };
            
        }

        public ReportBranchParameterModel Parameter { get; set; }
        public ReportBranchSecretModel Secret { get; set; }

        public async Task AddParameterAsync()
        {
            var response = await HttpClient.PostAsJsonAsync("api/reports/parameters", Parameter);
            Parameter = await response.Content.ReadFromJsonAsync<ReportBranchParameterModel>();
            Branch.Parameters.Add(Parameter);
            Parameter = new ReportBranchParameterModel() { BranchId = Branch.Id, InputType = "text" };
        }

        public async Task RemoveParameterAsync(ReportBranchParameterModel parameter)
        {
            await HttpClient.DeleteAsync("api/reports/parameters/" + parameter.Id);
            Branch.Parameters.Remove(parameter);
        }

        public async Task AddSecretAsync()
        {
            var response = await HttpClient.PostAsJsonAsync("api/reports/secrets", Secret);
            Secret = await response.Content.ReadFromJsonAsync<ReportBranchSecretModel>();
            Branch.Secrets.Add(Secret);
            Secret = new ReportBranchSecretModel() { BranchId = Branch.Id };
        }

        public async Task RemoveSecretAsync(ReportBranchSecretModel secret)
        {
            await HttpClient.DeleteAsync("api/reports/secrets/" + secret.Id);
            Branch.Secrets.Remove(secret);
        }

        private async Task PostReportBranchAsync(ReportBranchModel branch)
        {
            var response = await HttpClient.PostAsJsonAsync("api/reports/branches", branch);
            Report.Branches.Add(await response.Content.ReadFromJsonAsync<ReportBranchModel>());
        }

        private async Task PutReportBranchAsync(ReportBranchModel branch)
        {
            await HttpClient.PutAsJsonAsync("api/reports/branches", branch);
        }

        private async Task EditReportBranchAsync(ReportBranchModel branch)
        {
            await Modal.UpdateAsync(branch);
        }
        private async Task AddReportBranchAsync() => await Modal.CreateAsync();
    }
}
