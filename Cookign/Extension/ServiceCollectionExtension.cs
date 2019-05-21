
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cookign.Extension
{
    public static class ServiceCollectionExtension
    {
        public static void AddCookign(this IServiceCollection services, Action<CookignAuthenticationOptions> configureOptions, Action<DataProtectionOptions> dataproteccionOptions)
        {

            
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            if(dataproteccionOptions is null)
            {
                services.AddDataProtection();
            }
            else
            {
                services.AddDataProtection(dataproteccionOptions);
            }
            
            services.AddSingleton<IPostConfigureOptions<CookignAuthenticationOptions>, CookignAuthenticationPostConfigureOptions>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookignConstants.AuthenticationScheme;
                options.DefaultSignInScheme = CookignConstants.AuthenticationScheme;
                options.DefaultChallengeScheme = CookignConstants.AuthenticationScheme;
            }).AddScheme<CookignAuthenticationOptions, CookignAuthenticationHandler>(CookignConstants.AuthenticationScheme, configureOptions);
        }

        public static void AddCookign(this IServiceCollection services, Action<CookignAuthenticationOptions> configureOptions)
        {
            services.AddCookign(configureOptions, null);
        }

        public static void AddCookign(this IServiceCollection services)
        {
            CookignAuthenticationOptions options = new CookignAuthenticationOptions();

            services.AddCookign( x => x = options);
        }

        
    }
}
