using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cookign
{
    public class CookignAuthenticationOptions : AuthenticationSchemeOptions
    {

        public TimeSpan ExpireTimeSpan { get; set; } = new TimeSpan(1, 0, 0, 0, 0);

        public PathString LoginPath { get; set; }

        public PathString LogoutPath { get; set; }

        public bool ValidateIssuer { get; set; } = true;

        public bool ValidateAudience { get; set; } = true;

        public bool ValidateIpPublic { get; set; } = true;

        public string SecretKey { get; set; }

        public bool CreateClaimsCookie { get; set; } = true;

        public string CookieTokenName { get; set; } = "access_token";


        public string CookieClaimsName { get; set; } = "identity";

        public bool Secure { get; set; } = true;

        public SameSiteMode SameSiteMode { get; set; } = SameSiteMode.Strict;

        public bool HttpOnly { get; set; } = true ;

        internal CookingSettings CookingSettings { get; set; } = new CookingSettings();
    }
}
