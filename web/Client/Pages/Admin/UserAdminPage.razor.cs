using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Shearlegs.Web.Shared.Constants;
using Shearlegs.Web.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Shearlegs.Web.Client.Pages.Admin
{
    [Authorize(Roles = RoleConstants.AdminRoleId)]
    public partial class UserAdminPage
    {
        [Parameter]
        public int UserId { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        public UserModel User { get; set; }
        public List<ReportModel> Reports { get; set; }

        protected override async Task OnInitializedAsync()
        {
            User = await HttpClient.GetFromJsonAsync<UserModel>("api/users/" + UserId);
            Reports = await HttpClient.GetFromJsonAsync<List<ReportModel>>("api/reports");
        }

        private async Task UpdateUserAsync(UserModel user)
        {
            await HttpClient.PutAsJsonAsync("api/users", user);
            user.Password = string.Empty;
        }

        private async Task AddReportUser(ReportModel report)
        {
            var reportUser = new ReportUserModel()
            {
                ReportId = report.Id,
                UserId = User.Id
            };
            var response = await HttpClient.PostAsJsonAsync("api/reports/users", reportUser);
            reportUser = await response.Content.ReadFromJsonAsync<ReportUserModel>();
            User.ReportUsers.Add(reportUser);
        }

        private async Task DeleteReportUser(ReportModel report)
        {
            var reportUser = User.ReportUsers.First(x => x.ReportId == report.Id);
            await HttpClient.DeleteAsync($"api/reports/users/{reportUser.Id}");
            User.ReportUsers.Remove(reportUser);
        }
    }
}
