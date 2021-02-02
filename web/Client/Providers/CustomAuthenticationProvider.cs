using Microsoft.AspNetCore.Components.Authorization;
using Shearlegs.Web.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shearlegs.Web.Client.Providers
{
    public class CustomAuthenticationProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;

        public CustomAuthenticationProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public UserModel User { get; set; }

        public async Task UpdateUserAsync()
        {
            var response = await httpClient.GetAsync("api/users/me");
            if (response.IsSuccessStatusCode)
            {
                User = await response.Content.ReadFromJsonAsync<UserModel>();
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (User == null)
            {
                await UpdateUserAsync();
            }                

            ClaimsIdentity identity;
            if (User == null)
            {
                identity = new ClaimsIdentity();   
            } else
            {
                identity = new ClaimsIdentity(new[] 
                { 
                    new Claim(ClaimTypes.NameIdentifier, User.Id.ToString()),
                    new Claim(ClaimTypes.Name, User.Name),
                    new Claim(ClaimTypes.Role, User.Role)
                });
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
