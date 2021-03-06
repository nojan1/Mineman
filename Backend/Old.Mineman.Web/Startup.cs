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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Swagger;
using Mineman.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;

namespace WebApplicationBasic
{
    public class Startup
    {
        private string secretKey = "keykeykeykeykeykeykeykeykeykeykeykeykeykeykey";
        private SymmetricSecurityKey SigningKey { get => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)); }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

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

            
            var tokenValidationParameters = new TokenValidationParameters
            {
                //// The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKey,

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                   
                options.TokenValidationParameters = tokenValidationParameters;
            });

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
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Mineman API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[0]}
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
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
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            //Remap "sub" to be name, original is nameidentifier. Needed for default identity unboxing
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["sub"] = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
            app.UseAuthentication();

            var options = new TokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256),
                Expiration = TimeSpan.FromHours(1)
            };
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
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
            dockerClient.System.PingAsync().Wait();
        }
    }
}
