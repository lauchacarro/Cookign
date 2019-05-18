
using Microsoft.AspNetCore.Authentication;
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
        public static void AddCookign(this IServiceCollection services, Action<CookignAuthenticationOptions> configureOptions)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDataProtection();

            


            services.AddSingleton<IPostConfigureOptions<CookignAuthenticationOptions>, CookignAuthenticationPostConfigureOptions>();
            //services.AddTransient<ICookignAuthenticationService, CookignAuthenticationService>();

            services.AddAuthentication(nameof(Cookign)).AddScheme<CookignAuthenticationOptions, CookignAuthenticationHandler>(nameof(Cookign), configureOptions);

            //services.AddSingleton<IAuthenticationSignInHandler, CookignAuthenticationSignInHandler>();




        }

        public static void AddCookign(this IServiceCollection services)
        {
            CookignAuthenticationOptions options = new CookignAuthenticationOptions();

            services.AddCookign( x => x = options);
        }

        
    }
}
