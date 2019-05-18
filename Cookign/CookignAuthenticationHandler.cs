
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

        public CookignAuthenticationHandler( IDataProtectionProvider dataProtectionProvider, IOptionsMonitor<CookignAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }



        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Cookies.Count(x => x.Key == Options.CookieTokenName) != 1)
            {
                return AuthenticateResult.NoResult();
            }

            string cookieValue = Request.Cookies.Single(x => x.Key == Options.CookieTokenName).Value;

            if (string.IsNullOrWhiteSpace(cookieValue))
            {
                return AuthenticateResult.NoResult();
            }

            AuthenticationTicket ticket = CreateProtector().Unprotect(cookieValue);

            if(ticket == null)
            {
                return AuthenticateResult.NoResult();
            }

            ClaimsIdentity claimsIdentity = ticket.Principal.Identities.SingleOrDefault(x => x.AuthenticationType == Constants.CookingIdentitySetting);
            if(claimsIdentity == null)
            {
                return AuthenticateResult.NoResult();
            }

            if (Options.ValidateIssuer)
            {
                string issuer = claimsIdentity.Claims.SingleOrDefault(x => x.Type == Constants.ClaimIssuer).Value;
                if(issuer != Options.CookingSettings.Issuer)
                {
                    return AuthenticateResult.NoResult();
                }
            }

            if (Options.ValidateAudience)
            {
                string audience = claimsIdentity.Claims.SingleOrDefault(x => x.Type == Constants.ClaimAudience).Value;
                if (audience != Options.CookingSettings.Audience)
                {
                    return AuthenticateResult.NoResult();
                }
            }

            if (Options.ValidateIpPublic)
            {
                string remoteIp = claimsIdentity.Claims.SingleOrDefault(x => x.Type == Constants.ClaimRemoteIp).Value;
                if (remoteIp != Context.Connection.RemoteIpAddress.ToString())
                {
                    return AuthenticateResult.NoResult();
                }
            }

            return AuthenticateResult.Success(ticket);
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
                claims.Add(new Claim(Constants.ClaimIssuer, Options.CookingSettings.Issuer));
            }
            if (Options.ValidateAudience)
            {
                claims.Add(new Claim(Constants.ClaimAudience, Options.CookingSettings.Audience));
            }
            if (Options.ValidateIpPublic)
            {
                claims.Add(new Claim(Constants.ClaimRemoteIp, Context.Connection.RemoteIpAddress.ToString()));
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, Constants.CookingIdentitySetting);

            user.AddIdentity(claimsIdentity);

            AuthenticationTicket ticket = new AuthenticationTicket(user, properties, Scheme.Name);

            string valueCookie = CreateProtector().Protect(ticket);

            CookieOptions options = new CookieOptions
            {
                HttpOnly = Options.HttpOnly,
                SameSite = Options.SameSiteMode,
                Secure = Options.Secure,
                Expires = DateTime.Now + Options.ExpireTimeSpan
            };


            Response.Cookies.Append(Options.CookieTokenName, valueCookie, options);


        }

        protected override async Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            Response.Cookies.Delete(Options.CookieTokenName);
        }

        private ISecureDataFormat<AuthenticationTicket> CreateProtector()
        {
            IDataProtector protector = _dataProtectionProvider.CreateProtector(Options.CookingSettings.SecretKey);
            ISecureDataFormat<AuthenticationTicket> ticketDataFormat = new TicketDataFormat(protector);
            return ticketDataFormat;
        }
    }
}
