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
using Mineman.WorldParsing;
using Mineman.WorldParsing.MapTools;
using Mineman.Service.Helpers;
using Microsoft.AspNetCore.Http.Features;
using Mineman.Common.Models.Configuration;
using Mineman.WorldParsing.MapTools.Models;
using Mineman.Service.Models.Configuration;
using Mineman.Web.Filters;

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
            {
                options.UseSqlite(Configuration.GetConnectionString("MainDatabase"));
            });

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
                var dockerOptions = service.GetService<IOptions<DockerOptions>>();

                return new DockerClientConfiguration(new Uri(
                        dockerOptions.Value.DockerHost
                    //Configuration.GetSection("DockerOptions").GetValue<string>("DockerHost")
                    )).CreateClient(Version.Parse("1.24"));
            });
            services.AddScoped<IServerRepository, ServerRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IModRepository, ModRepository>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();

            services.AddScoped<IServerManager, ServerManager>();
            services.AddScoped<IImageManager, ImageManager>();
            services.AddTransient<IMinecraftServerQuery, MinecraftServerQuery>();
            services.AddSingleton<IConnectionPool, ConnectionPool>();
            services.AddSingleton<IRemoteImageRepository, RemoteImageRepository>();

            services.AddTransient<ITextureProvider, TextureProvider>(service =>
            {
                var mapGenerationOptions = service.GetService<IOptions<TextureOptions>>();
                return new TextureProvider(mapGenerationOptions.Value);
            });
            services.AddTransient<IWorldParserFactory, WorldParserFactory>();
            services.AddTransient<IMapRendererFactory, MapRendererFactory>();
            services.AddScoped<MapGenerationService>();
            services.AddScoped<WorldInfoService>();
            services.AddScoped<BackgroundService>();

            services.Configure<DockerOptions>(Configuration.GetSection("DockerOptions"));
            services.Configure<PathOptions>(Configuration.GetSection("PathOptions"));
            services.Configure<BackgroundServiceOptions>(Configuration.GetSection("BackgroundServiceOptions"));
            services.Configure<ServerCommunicationOptions>(Configuration.GetSection("ServerCommunicationOptions"));
            services.Configure<TextureOptions>(Configuration.GetSection("TextureOptions"));
            services.Configure<RemoteImageOptions>(Configuration.GetSection("RemoteImageOptions"));

            // Add framework services.
            services.AddMvc(x =>
            {
                x.Filters.Add(typeof(GlobalExceptionLoggerFilter));
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              ILoggerFactory loggerFactory,
                              DatabaseContext context,
                              BackgroundService service,
                              IOptions<PathOptions> pathOptions,
                              UserManager<ApplicationUser> userManager,
                              IDockerClient dockerClient)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
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
                Expiration = TimeSpan.FromHours(1)
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
            //context.Database.Migrate(); TODO: Support migrations
            EnsureFoldersCreated(env, pathOptions.Value);
            EnsureAdminUserExists(userManager);

            try
            {
                StartupTest(dockerClient);
                service.Start();
            }
            catch (Exception ex)
            {
                loggerFactory.CreateLogger<Startup>()
                    .LogCritical(new EventId(), ex, "Startup test failed, refusing to start service");
            }
        }

        private void EnsureAdminUserExists(UserManager<ApplicationUser> userManager)
        {
            if (userManager.Users.Count() == 0)
            {
                var user = new ApplicationUser { UserName = "admin" };
                var result = userManager.CreateAsync(user, "admin").Result;

                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create initial user! {string.Join(",", result.Errors.Select(e => e.Code))}");
                }
            }
        }

        private void EnsureFoldersCreated(IHostingEnvironment env, PathOptions pathOptions)
        {
            void createDirectory(string path)
            {
                var fullPath = env.BuildPath(path);
                Directory.CreateDirectory(fullPath);
            }

            createDirectory(pathOptions.WorldDirectory);
            createDirectory(pathOptions.ServerPropertiesDirectory);
            createDirectory(pathOptions.ModDirectory);
            createDirectory(pathOptions.ImageZipFileDirectory);
        }

        private void StartupTest(IDockerClient dockerClient)
        {
            dockerClient.Miscellaneous.PingAsync().Wait();
        }
    }
}
