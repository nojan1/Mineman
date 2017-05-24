using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Mineman.Common.Database;
using Docker.DotNet;
using Mineman.Service;
using Mineman.Service.Repositories;
using Mineman.Service.Managers;
using Mineman.Common.Models;
using Microsoft.Extensions.Options;
using System.IO;
using Mineman.Service.MinecraftQuery;
using Mineman.Common.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Mineman.Web.Middleware;
using Mineman.Web.Models;
using Mineman.Web.Helpers;
using System.IdentityModel.Tokens.Jwt;
using Mineman.Service.Rcon;

namespace WebApplicationBasic
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("MainDatabase")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

            services.AddTransient<IDockerClient>(service =>
            {
                return new DockerClientConfiguration(new Uri(
                        Configuration.GetValue<string>("DockerHost")
                    )).CreateClient(Version.Parse("1.24"));
            });
            services.AddTransient<IServerRepository, ServerRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<IModRepository, ModRepository>();
            services.AddTransient<IWorldRepository, WorldRepository>();

            services.AddTransient<IServerManager, ServerManager>();
            services.AddTransient<IImageManager, ImageManager>();
            services.AddTransient<IMinecraftServerQuery, MinecraftServerQuery>();
            services.AddTransient<IConnectionPool, ConnectionPool>();

            services.AddTransient<BackgroundService>();

            services.Configure<Mineman.Common.Models.Configuration>(Configuration);

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IHostingEnvironment env, 
                              ILoggerFactory loggerFactory, 
                              DatabaseContext context, 
                              BackgroundService service,
                              IOptions<Configuration> configuration,
                              UserManager<ApplicationUser> userManager)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            var secretKey = "keykeykeykeykeykeykeykeykeykeykeykeykeykeykey";
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                //// The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            //Remap "sub" to be name, original is nameidentifier. Needed for default identity unboxing
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["sub"] = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AuthenticationScheme = "Identity.Application",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters,
            });

            var options = new TokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            context.Database.EnsureCreated();
            EnsureFoldersCreated(env, configuration.Value);
            EnsureAdminUserExists(userManager);

            service.Start();
        }

        private void EnsureAdminUserExists(UserManager<ApplicationUser> userManager)
        {
            if(userManager.Users.Count() == 0)
            {
                var user = new ApplicationUser { UserName = "admin" };
                var result = userManager.CreateAsync(user, "admin").Result;

                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create initial user! {string.Join(",", result.Errors.Select(e => e.Code))}");
                }
            }
        }

        private void EnsureFoldersCreated(IHostingEnvironment env, Configuration configuration)
        {
            Action<string> createDirectory = (path) =>
            {
                var fullPath = Path.Combine(env.ContentRootPath, path);
                Directory.CreateDirectory(fullPath);
            };

            createDirectory(configuration.WorldDirectory);
            createDirectory(configuration.ServerPropertiesDirectory);
            createDirectory(configuration.ModDirectory);
            createDirectory(configuration.ImageZipFileDirectory);
        }
    }
}
