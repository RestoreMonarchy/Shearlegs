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
    public partial class UsersAdminPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public List<UserModel> Users { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Users = await HttpClient.GetFromJsonAsync<List<UserModel>>("api/users");
        }

        private void GoToUser(UserModel user)
        {
            NavigationManager.NavigateTo("/admin/users/" + user.Id);
        }
    }
}
