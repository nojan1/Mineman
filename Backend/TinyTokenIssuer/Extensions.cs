using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using TinyTokenIssuer.Models;

namespace TinyTokenIssuer
{
    public static class IServiceCollectionExtensions
    {
        public static WithAddAuthentication AddTinyTokenIssuer(this IServiceCollection services, Action<TinyTokenIssuerConfig> configCallback)
        {
            var config = new TinyTokenIssuerConfig();
            configCallback(config);

            services.AddSingleton((_) => config);
            services.AddSingleton<TokenSigningKeyProvider>();

            return new WithAddAuthentication(config, services);
        }

    }

    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTinyTokenIssuer<TProfile>(this IApplicationBuilder app) where TProfile : BaseTokenProperties
        {
            app.UseMiddleware<TokenIssuerMiddleware<TProfile>>();

            return app;
        }
    }

    public class WithAddAuthentication
    {
        private readonly TinyTokenIssuerConfig _config;
        private readonly IServiceCollection _services;

        public WithAddAuthentication(TinyTokenIssuerConfig config, IServiceCollection services)
        {
            _config = config;
            _services = services;
        }

        //public void WithAuthentication()
        //{
        //    _services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //        .AddJwtBearer(options =>
        //        {
        //            options.Authority = AuthProperties.Issuer;
        //            options.Audience = AuthProperties.Audience;
        //        });
        //}
        public void WithConfig(Action<TinyTokenIssuerConfig> callback)
        {
            callback(_config);
        }
    }
}
