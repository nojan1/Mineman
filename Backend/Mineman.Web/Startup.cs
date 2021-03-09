using Docker.DotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using Mineman.Common.Models.Configuration;
using Mineman.Service;
using Mineman.Service.Managers;
using Mineman.Service.MinecraftQuery;
using Mineman.Service.Models.Configuration;
using Mineman.Service.Rcon;
using Mineman.Service.Repositories;
using Mineman.Web.Auth;
using Mineman.Web.Helpers;
using Mineman.Web.Hubs;
using Mineman.WorldParsing;
using Mineman.WorldParsing.MapTools;
using Mineman.WorldParsing.MapTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TinyTokenIssuer;
using TinyTokenIssuer.Interfaces;

namespace Mineman.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("MainDatabase"));
            });

            services.AddScoped<IProfileService<TokenUser>, UserProfileService>();
            services.AddTinyTokenIssuer(config =>
            {
                config.Issuer = AuthProperties.Issuer;
                config.Audience = AuthProperties.Audience;
            }).WithConfig(config =>
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           IssuerSigningKey = config.SigningKey,
                           ValidIssuer = config.Issuer,
                           ValidAudience = config.Audience
                       };
                   });
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

            services.AddSignalR();
            services.AddControllers();
            ConfiureDependencies(services);

            services.AddSwaggerGen();
            RegisterConfigurationOptionModels(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              ILoggerFactory loggerFactory,
                              DatabaseContext context,
                              Service.BackgroundService service,
                              IOptions<PathOptions> pathOptions,
                              UserManager<ApplicationUser> userManager,
                              IDockerClient dockerClient)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });

            app.UseCors(builder =>
            {
                //TODO: Actually set origin
                //var origin = Configuration.GetValue<string>("FrontendIntegrationOptions::CorsOrigin");
                //if (!string.IsNullOrEmpty(origin))

                builder.AllowAnyHeader();
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mineman API V1");
            });

            app.UseRouting();

            app.UseTinyTokenIssuer<TokenUser>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<InfoHub>("/info");
            });

            //context.Database.EnsureCreated();
            context.Database.Migrate();
            EnsureFoldersCreated(env, pathOptions.Value);
            EnsureAdminUserExists(userManager);

            try
            {
                //StartupTest(dockerClient);
                service.Start();
            }
            catch (Exception ex)
            {
                loggerFactory.CreateLogger<Startup>()
                    .LogCritical(new EventId(), ex, "Startup test failed, refusing to start service");
            }
        }

        private void RegisterConfigurationOptionModels(IServiceCollection services)
        {
            services.Configure<DockerOptions>(Configuration.GetSection("DockerOptions"));
            services.Configure<PathOptions>(Configuration.GetSection("PathOptions"));
            services.Configure<BackgroundServiceOptions>(Configuration.GetSection("BackgroundServiceOptions"));
            services.Configure<ServerCommunicationOptions>(Configuration.GetSection("ServerCommunicationOptions"));
            services.Configure<TextureOptions>(Configuration.GetSection("TextureOptions"));
            services.Configure<RemoteImageOptions>(Configuration.GetSection("RemoteImageOptions"));
        }

        private static void ConfiureDependencies(IServiceCollection services)
        {
            services.AddTransient<IDockerClient>(service =>
            {
                var dockerVersion = Version.Parse("1.24");
                var dockerOptions = service.GetService<IOptions<DockerOptions>>();

                if (!string.IsNullOrEmpty(dockerOptions.Value.DockerHost))
                    return new DockerClientConfiguration(new Uri(
                            dockerOptions.Value.DockerHost
                        )).CreateClient(dockerVersion);

                return new DockerClientConfiguration().CreateClient(dockerVersion);
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
            services.AddScoped<Service.BackgroundService>();

            services.AddTransient<IInfoClient, InfoHubClient>();
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

        private void EnsureFoldersCreated(IWebHostEnvironment env, PathOptions pathOptions)
        {
            void createDirectory(string path)
            {
                var fullPath = Path.Combine(env.ContentRootPath, path);
                Directory.CreateDirectory(fullPath);
            }

            createDirectory(pathOptions.WorldDirectory);
            createDirectory(pathOptions.WorldDirectory + "/map");
            createDirectory(pathOptions.WorldDirectory + "/info");
            createDirectory(pathOptions.ServerPropertiesDirectory);
            createDirectory(pathOptions.ModDirectory);
            createDirectory(pathOptions.ImageZipFileDirectory);
        }
    }
}
