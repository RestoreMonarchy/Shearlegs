using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shearlegs.Web.Server.Repositories;
using Shearlegs.Web.Shared.Constants;
using Shearlegs.Web.Shared.Models;
using Shearlegs.Web.Shared.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shearlegs.Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersRepository usersRepository;

        public UsersController(UsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            return Ok(await usersRepository.GetUsersAsync());
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUsersAsync(int userId)
        {
            return Ok(await usersRepository.GetUserAsync(userId));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPost]
        public async Task<IActionResult> PostUserAsync([FromBody] UserModel user)
        {
            return Ok(await usersRepository.AddUserAsync(user));
        }

        [Authorize(Roles = RoleConstants.AdminRoleId)]
        [HttpPut]
        public async Task<IActionResult> PutUserAsync([FromBody] UserModel user)
        {
            await usersRepository.UpdateUserAsync(user);
            return Ok();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMeUserAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            return Ok(await usersRepository.GetUserAsync(int.Parse(User.Identity.Name)));
        }

        [ResponseCache(NoStore = true, Duration = 0)]
        [HttpPost("~/signin")]
        public async Task<IActionResult> SignInAsync([FromBody] LoginParams loginParams)
        {
            var user = await usersRepository.GetUserAsync(loginParams.Name, loginParams.Password);

            if (user == null)
            {
                return BadRequest();
            }

            if (loginParams.ReturnUrl == null)
            {
                loginParams.ReturnUrl = "/";
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(24),
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                RedirectUri = loginParams.ReturnUrl
            };

            return SignIn(claimsPrincipal, authProperties);
        }

        [ResponseCache(NoStore = true, Duration = 0)]
        [HttpGet("~/signout"), HttpPost("~/signout")]
        public IActionResult SignOutAsync()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
