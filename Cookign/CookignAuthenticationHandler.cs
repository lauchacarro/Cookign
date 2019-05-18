
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Cookign
{
    public class CookignAuthenticationHandler : SignInAuthenticationHandler<CookignAuthenticationOptions>
    {

        private readonly IDataProtectionProvider _dataProtectionProvider;

        private readonly IActionContextAccessor _accessor;

        public CookignAuthenticationHandler(IActionContextAccessor accessor, IDataProtectionProvider dataProtectionProvider, IOptionsMonitor<CookignAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _accessor = accessor;
        }



        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            

            return AuthenticateResult.NoResult();
        }

        protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            properties = properties ?? new AuthenticationProperties();

            List<Claim> claims = new List<Claim>();
            if (Options.ValidateIssuer)
            {
                claims.Add(new Claim("Issuer", Options.CookingSettings.Issuer));
            }
            if (Options.ValidateAudience)
            {
                claims.Add(new Claim("Audience", Options.CookingSettings.Audience));
            }
            if (Options.ValidateIpPublic)
            {
                claims.Add(new Claim("RemoteIp", Context.Connection.RemoteIpAddress.ToString()));
            }
            List<ClaimsIdentity> identities = user.Identities.ToList();

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Settings");

            user.AddIdentity(claimsIdentity);





            AuthenticationTicket ticket = new AuthenticationTicket(user, properties, Scheme.Name);

            IDataProtector provider =_dataProtectionProvider.CreateProtector(Options.CookingSettings.SecretKey);
            
            TicketDataFormat ticketDataFormat = new TicketDataFormat(provider);
            string valueCookie = ticketDataFormat.Protect(ticket);

            CookieOptions options = new CookieOptions
            {
                HttpOnly = Options.HttpOnly,
                SameSite = Options.SameSiteMode,
                Secure = Options.Secure,
                Expires = DateTime.Now + Options.ExpireTimeSpan
            };


            Response.Cookies.Append(Options.CookieTokenName, valueCookie, options);

  
        }

        protected override Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }
    }
}
