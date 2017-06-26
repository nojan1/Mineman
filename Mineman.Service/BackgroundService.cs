using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Service.Helpers;
using Mineman.Service.Managers;
using Mineman.Service.Models.Configuration;
using Mineman.Service.Rcon;
using Mineman.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service
{
    public class BackgroundService
    {
        private readonly ILogger<BackgroundService> _logger;
        private readonly IConnectionPool _connectionPool;
        private readonly MapGenerationService _mapGenerationService;
        private readonly WorldInfoService _worldInfoService;
        private readonly IServiceScopeFactory _serviceFactory;
        private readonly BackgroundServiceOptions _backgroundServiceOptions;

        private Task _backgroundTasks;
        private DateTimeOffset _nextBackgroundTaskRun;

        public BackgroundService(ILogger<BackgroundService> logger,
                                 IConnectionPool connectionPool,
                                 MapGenerationService mapGenerationService,
                                 IOptions<BackgroundServiceOptions> options,
                                 WorldInfoService worldInfoService,
                                 IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            _logger = logger;
            _connectionPool = connectionPool;
            _mapGenerationService = mapGenerationService;
            _worldInfoService = worldInfoService;
            _backgroundServiceOptions = options.Value;

            _nextBackgroundTaskRun = DateTimeOffset.Now;
        }

        public void Start()
        {
            _logger.LogInformation("Background service starting up");

            try
            {
                RemoveUnusedResourcesFromDocker().Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, "Error during startup. Unable to removed unused docker resources.");
            }

            Task.Run(() =>
            {
                WorkingLoop().Wait();
            });
        }

        private async Task RemoveUnusedResourcesFromDocker()
        {
            _logger.LogInformation("Checking docker for containers/images not present in database");

            using (var scope = _serviceFactory.CreateScope())
            {
                await scope.ServiceProvider.GetService<IServerManager>().RemoveUnusedContainers();
                await scope.ServiceProvider.GetService<IImageManager>().RemoveUnsuedImages();
            }

        }

        private async Task WorkingLoop()
        {
            _logger.LogInformation("Background service entered working loop");

            while (true)
            {
                try
                {
                    using (var scope = _serviceFactory.CreateScope())
                    {
                        await InvalidateMissingImages(scope);
                        await CreateImages(scope);
                        await StartIdleContainers(scope);

                        _connectionPool.DisposeConnectionsOlderThen(TimeSpan.FromMinutes(1));

                        if (_backgroundServiceOptions.EnableBackgroundWorldProcessing && (_backgroundTasks == null || _backgroundTasks.IsCompleted) && _nextBackgroundTaskRun <= DateTimeOffset.Now)
                        {
                            _backgroundTasks = Task.Run(() =>
                            {
                                _mapGenerationService.GenerateForAllWorlds().Wait();
                                _worldInfoService.GenerateForAllWorlds().Wait();
                            })
                            .ContinueWith((task) =>
                            {
                                _nextBackgroundTaskRun = DateTimeOffset.Now + _backgroundServiceOptions.WorldProcessingInterval;
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(), e, $"BackgroundService: Error in working loop");
                }

                await Task.Delay(_backgroundServiceOptions.WorkingLoopSleepInterval);
            }
        }

        private async Task InvalidateMissingImages(IServiceScope scope)
        {
            try
            {
                await scope.ServiceProvider.GetService<IImageManager>().InvalidateMissingImages();
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"BackgroundService: Error when invalidating missing images");
            }
        }

        private async Task CreateImages(IServiceScope scope)
        {
            _logger.LogDebug("BackgroundService: About to create pending images");

            var imageManager = scope.ServiceProvider.GetService<IImageManager>();
            var imageRepository = scope.ServiceProvider.GetService<IImageRepository>();

            foreach (var image in imageRepository.GetImages().Where(i => i.BuildStatus == null || !i.BuildStatus.BuildSucceeded))
            {
                try
                {
                    await imageManager.CreateImage(image);
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(), e, $"BackgroundService: Error when creating image");
                }
            }

        }

        private async Task StartIdleContainers(IServiceScope scope)
        {
            _logger.LogDebug("BackgroundService: About to start idle servers");

            var serverManager = scope.ServiceProvider.GetService<IServerManager>();
            var serverRepository = scope.ServiceProvider.GetService<IServerRepository>();

            foreach (var server in (await serverRepository.GetServers()).Where(s => !s.IsAlive && s.Server.ShouldBeRunning))
            {
                try
                {
                    await serverManager.Start(server.Server);
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(), e, $"BackgroundService: Error when starting idle server");
                }
            }
        }
    }
}
