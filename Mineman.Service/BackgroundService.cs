using Microsoft.Extensions.Logging;
using Mineman.Service.Helpers;
using Mineman.Service.Managers;
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
        private readonly IServerManager _serverManager;
        private readonly IImageManager _imageManager;
        private readonly IImageRepository _imageRepository;
        private readonly IServerRepository _serverRepository;
        private readonly ILogger<BackgroundService> _logger;
        private readonly IConnectionPool _connectionPool;
        private readonly MapGenerationService _mapGenerationService;
        private readonly WorldInfoService _worldInfoService;

        private Task _backgroundTasks;
        private DateTimeOffset _nextBackgroundTaskRun;

        public BackgroundService(IServerManager serverManager,
                                 IImageManager imageManager,
                                 IImageRepository imageRepository,
                                 IServerRepository serverRepository,
                                 ILogger<BackgroundService> logger,
                                 IConnectionPool connectionPool,
                                 MapGenerationService mapGenerationService,
                                 WorldInfoService worldInfoService)
        {
            _serverManager = serverManager;
            _imageManager = imageManager;
            _imageRepository = imageRepository;
            _serverRepository = serverRepository;
            _logger = logger;
            _connectionPool = connectionPool;
            _mapGenerationService = mapGenerationService;
            _worldInfoService = worldInfoService;

            _nextBackgroundTaskRun = DateTimeOffset.Now;
        }

        public void Start()
        {
            _logger.LogInformation("Background service starting up");

            RemoveUnusedResourcesFromDocker().Wait();

            Task.Run(() =>
            {
                WorkingLoop().Wait();
            });
        }

        private async Task RemoveUnusedResourcesFromDocker()
        {
            _logger.LogInformation("Checking docker for containers/images not present in database");

            await _serverManager.RemoveUnusedContainers();
            await _imageManager.RemoveUnsuedImages();
        }

        private async Task WorkingLoop()
        {
            _logger.LogInformation("Background service entered working loop");

            while (true)
            {
                try
                {
                    await InvalidateMissingImages();
                    await CreateImages();
                    await StartIdleContainers();

                    _connectionPool.DisposeConnectionsOlderThen(TimeSpan.FromMinutes(1));

                    if ((_backgroundTasks == null || _backgroundTasks.IsCompleted) && _nextBackgroundTaskRun <= DateTimeOffset.Now)
                    {
                        _backgroundTasks = Task.Run(() =>
                        {
                            _mapGenerationService.GenerateForAllWorlds().Wait();
                            _worldInfoService.GenerateForAllWorlds().Wait();
                        })
                        .ContinueWith((task) =>
                        {
                            _nextBackgroundTaskRun = DateTimeOffset.Now + TimeSpan.FromHours(1);
                        });
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(), e, $"BackgroundService: Error in working loop");
                }

                await Task.Delay(TimeSpan.FromMinutes(2));
            }
        }

        private async Task InvalidateMissingImages()
        {
            try
            {
                await _imageManager.InvalidateMissingImages();
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"BackgroundService: Error when invalidating missing images");
            }
        }

        private async Task CreateImages()
        {
            _logger.LogDebug("BackgroundService: About to create pending images");

            foreach (var image in _imageRepository.GetImages().Where(i => i.BuildStatus == null || !i.BuildStatus.BuildSucceeded))
            {
                try
                {
                    await _imageManager.CreateImage(image);
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(), e, $"BackgroundService: Error when creating image");
                }
            }

        }

        private async Task StartIdleContainers()
        {
            _logger.LogDebug("BackgroundService: About to start idle servers");

            foreach (var server in (await _serverRepository.GetServers()).Where(s => !s.IsAlive && s.Server.ShouldBeRunning))
            {
                try
                {
                    await _serverManager.Start(server.Server);
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(), e, $"BackgroundService: Error when starting idle server");
                }
            }
        }
    }
}
