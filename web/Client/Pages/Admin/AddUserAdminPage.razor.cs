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
    public partial class AddUserAdminPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private async Task AddUserAsync(UserModel user)
        {
            var response = await HttpClient.PostAsJsonAsync("api/users", user);
            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadFromJsonAsync<UserModel>();
                NavigationManager.NavigateTo("/admin/users/" + user.Id);
            }
        }
    }
}
