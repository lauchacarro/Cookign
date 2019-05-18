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
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "Laucha")
                    };
            ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(principal);
            return Ok();
        }

        public async Task<IActionResult> Out ()
        {

            await HttpContext.SignOutAsync();
            return Ok();
        }
    }
}