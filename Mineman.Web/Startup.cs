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

            services.AddTransient<IDockerClient>(service =>
            {
                return new DockerClientConfiguration(new Uri(
                        Configuration.GetValue<string>("DockerHost")
                    )).CreateClient(Version.Parse("1.24"));
            });
            services.AddTransient<IServerRepository, ServerRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<ModRepository, ModRepository>();
            services.AddTransient<IWorldRepository, WorldRepository>();

            services.AddTransient<IServerManager, ServerManager>();
            services.AddTransient<IImageManager, ImageManager>();
            services.AddTransient<IMinecraftServerQuery, MinecraftServerQuery>();

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
                              IOptions<Configuration> configuration)
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

            service.Start();
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
