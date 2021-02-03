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

        protected override async Task OnInitializedAsync()
        {
            User = await HttpClient.GetFromJsonAsync<UserModel>("api/users/" + UserId);
        }

        private async Task UpdateUserAsync(UserModel user)
        {
            await HttpClient.PutAsJsonAsync("api/users", user);
        }
    }
}
