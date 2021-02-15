using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Models.Configuration;
using Mineman.Service;
using Mineman.Service.Managers;
using Mineman.Service.MinecraftQuery;
using Mineman.Service.Models.Configuration;
using Mineman.Service.Rcon;
using Mineman.Service.Repositories;
using Mineman.WorldParsing;
using Mineman.WorldParsing.MapTools;
using Mineman.WorldParsing.MapTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            services.AddControllers();

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

            services.Configure<DockerOptions>(Configuration.GetSection("DockerOptions"));
            services.Configure<PathOptions>(Configuration.GetSection("PathOptions"));
            services.Configure<BackgroundServiceOptions>(Configuration.GetSection("BackgroundServiceOptions"));
            services.Configure<ServerCommunicationOptions>(Configuration.GetSection("ServerCommunicationOptions"));
            services.Configure<TextureOptions>(Configuration.GetSection("TextureOptions"));
            services.Configure<RemoteImageOptions>(Configuration.GetSection("RemoteImageOptions"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
