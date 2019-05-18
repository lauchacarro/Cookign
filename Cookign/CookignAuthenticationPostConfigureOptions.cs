using Cookign.Exception;
using Cookign.Message;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cookign
{
    public class CookignAuthenticationPostConfigureOptions : IPostConfigureOptions<CookignAuthenticationOptions>
    {

        private readonly IConfiguration Configuration;

        public CookignAuthenticationPostConfigureOptions(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void PostConfigure(string name, CookignAuthenticationOptions options)
        {

            var section = Configuration.GetSection(nameof(Cookign));
            if (!section.Exists())
            {
                throw new CookignException(ErrorMessagesEnum.SectionNotFound);
            }

            var sectionChildrens = section.GetChildren();

            if (options.ValidateIssuer)
            {
                if (!sectionChildrens.Any(x => x.Key == nameof(CookingSettings.Issuer) && !string.IsNullOrWhiteSpace(x.Value)))
                {
                    throw new CookignException(ErrorMessagesEnum.IssuerNotFound);
                }
                options.CookingSettings.Issuer = sectionChildrens.Single(x => x.Key == nameof(CookingSettings.Issuer)).Value;
            }

            if(options.ValidateAudience)
            {
                if (!sectionChildrens.Any(x => x.Key == nameof(CookingSettings.Audience) && !string.IsNullOrWhiteSpace(x.Value)))
                {
                    throw new CookignException(ErrorMessagesEnum.AudienceNotFound);
                }
                options.CookingSettings.Audience = sectionChildrens.Single(x => x.Key == nameof(CookingSettings.Audience)).Value;

            }

            if (!sectionChildrens.Any(x => x.Key == nameof(CookingSettings.SecretKey) && !string.IsNullOrWhiteSpace(x.Value)))
            {
                throw new CookignException(ErrorMessagesEnum.SecretKeyNotFound);
            }
            options.CookingSettings.SecretKey = sectionChildrens.Single(x => x.Key == nameof(CookingSettings.SecretKey)).Value;

        }
    }
}
