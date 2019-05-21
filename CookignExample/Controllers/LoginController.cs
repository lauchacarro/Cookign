using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace CookignExample.Controllers
{
    public class LoginController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Cookign")
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
            return Ok();
        }

        public async Task<IActionResult> Out()
        {

            await HttpContext.SignOutAsync();
            return Ok();
        }
    }
}