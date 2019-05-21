
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
using System.Net;
using System.Text;

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

            ClaimsIdentity claimsIdentity = ticket.Principal.Identities.SingleOrDefault(x => x.AuthenticationType == CookignConstants.CookingIdentitySetting);
            if(claimsIdentity == null)
            {
                return AuthenticateResult.NoResult();
            }

            if (Options.ValidateIssuer)
            {
                string issuer = ticket.Properties.GetParameter<string>(CookignConstants.Issuer);
                if(issuer != Options.CookingSettings.Issuer)
                {
                    return AuthenticateResult.NoResult();
                }
            }

            if (Options.ValidateAudience)
            {
                string audience = ticket.Properties.GetParameter<string>(CookignConstants.Audience);
                if (audience != Options.CookingSettings.Audience)
                {
                    return AuthenticateResult.NoResult();
                }
            }

            if (Options.ValidateIpPublic)
            {
                if (ticket.Properties.GetParameter<IPAddress>(CookignConstants.RemoteIp) == null)
                {
                    return AuthenticateResult.NoResult();
                }
                IPAddress address = ticket.Properties.GetParameter<IPAddress>(CookignConstants.RemoteIp);
                if(address.ToString() != Context.Connection.RemoteIpAddress.ToString())
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

            if (Options.ValidateIssuer)
            {
                if(string.IsNullOrWhiteSpace(properties.GetParameter<string>(CookignConstants.Issuer)))
                {
                    properties.SetParameter(CookignConstants.Issuer, Options.CookingSettings.Issuer);
                }
                
            }
            if (Options.ValidateAudience)
            {
                if (string.IsNullOrWhiteSpace(properties.GetParameter<string>(CookignConstants.Audience)))
                {
                    properties.SetParameter(CookignConstants.Audience, Options.CookingSettings.Audience);
                }
            }
            if (Options.ValidateIpPublic)
            {
                if (properties.GetParameter<IPAddress>(CookignConstants.RemoteIp) == null)
                {
                    properties.SetParameter(CookignConstants.RemoteIp, Context.Connection.RemoteIpAddress);
                }
            }

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

            if (Options.CreateCookieClaims)
            {
                options = new CookieOptions
                {
                    Expires = DateTime.Now + Options.ExpireTimeSpan
                };
                string claimsJson = Newtonsoft.Json.JsonConvert.SerializeObject(from c in user.Claims select new { c.Type, c.Value });
                string cookieClaimValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(claimsJson));
                Response.Cookies.Append(Options.CookieClaimsName, cookieClaimValue, options);
            }
        }

        protected override async Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            Response.Cookies.Delete(Options.CookieTokenName);
            Response.Cookies.Delete(Options.CookieClaimsName);
        }

        private ISecureDataFormat<AuthenticationTicket> CreateProtector()
        {
            IDataProtector protector = _dataProtectionProvider.CreateProtector(Options.CookingSettings.SecretKey);
            ISecureDataFormat<AuthenticationTicket> ticketDataFormat = new TicketDataFormat(protector);
            return ticketDataFormat;
        }
    }
}
