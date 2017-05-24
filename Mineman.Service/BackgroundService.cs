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

        public BackgroundService(IServerManager serverManager,
                                 IImageManager imageManager,
                                 IImageRepository imageRepository,
                                 IServerRepository serverRepository,
                                 ILogger<BackgroundService> logger,
                                 IConnectionPool connectionPool)
        {
            _serverManager = serverManager;
            _imageManager = imageManager;
            _imageRepository = imageRepository;
            _serverRepository = serverRepository;
            _logger = logger;
            _connectionPool = connectionPool;
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
                await StartIdleContainers();
                await CreateImages();
                _connectionPool.DisposeConnectionsOlderThen(TimeSpan.FromMinutes(1));

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }

        private async Task CreateImages()
        {
            try
            {
                foreach (var image in _imageRepository.GetImages().Where(i => i.BuildStatus == null))
                {
                    await _imageManager.CreateImage(image);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"BackgroundService: Error when creating images");
            }
        }

        private async Task StartIdleContainers()
        {
            try
            {
                foreach (var server in (await _serverRepository.GetServers()).Where(s => !s.IsAlive && s.Server.ShouldBeRunning))
                {
                    await _serverManager.Start(server.Server);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"BackgroundService: Error when starting idle servers");
            }
        }
    }
}
