using Microsoft.AspNetCore.Builder.Extensions;
using NCMENERGY.Options;

namespace NCMENERGY.Injections
{
    public static class ConfigurationInjections
    {
        public static void AddAppConfigurations(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<MailOptions>(configuration.GetSection("MailOptions"));
            services.Configure<Jwt>(configuration.GetSection("Jwt"));
            services.Configure<PaystackIntegration>(configuration.GetSection("PaystackIntegration"));

        }
    }
}
