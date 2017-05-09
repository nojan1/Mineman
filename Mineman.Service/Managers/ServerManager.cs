using Docker.DotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using Mineman.Common.Models;
using Mineman.Service.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mineman.Service.Managers
{
    public class ServerManager : IServerManager
    {
        private readonly DatabaseContext _context;
        private readonly IDockerClient _dockerClient;
        private readonly Configuration _configuration;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<ServerManager> _logger;

        public ServerManager(DatabaseContext context,
                             IDockerClient dockerClient,
                             IOptions<Configuration> configuration,
                             IHostingEnvironment environment,
                             ILogger<ServerManager> logger)
        {
            _context = context;
            _dockerClient = dockerClient;
            _configuration = configuration.Value;
            _environment = environment;
            _logger = logger;
        }

        public async Task<bool> Start(Server server)
        {
            try
            {
                _logger.LogInformation($"About to start server. ServerID: {server.ID}");

                if (string.IsNullOrEmpty(server.ContainerID) ||
                   await DockerQueryHelper.GetContainer(_dockerClient, server.ContainerID) == null)
                {
                    await CreateContainer(server);
                }

                var result = await _dockerClient.Containers.StartContainerAsync(server.ContainerID, new Docker.DotNet.Models.ContainerStartParameters
                {

                });

                if (!result)
                {
                    throw new Exception("Server failed to start");
                }

                _logger.LogInformation($"Server started. ServerID: {server.ID}");

                server.ShouldBeRunning = true;

                _context.Update(server);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"Error occurred when starting server. ServerID: {server.ID}");

                return false;
            }
        }

        public async Task<bool> Stop(Server server)
        {
            try
            {
                _logger.LogInformation($"About to stop server. ServerID: {server.ID}");

                var result = await _dockerClient.Containers.StopContainerAsync(server.ContainerID,
                new Docker.DotNet.Models.ContainerStopParameters
                {
                    WaitBeforeKillSeconds = 10
                },
                CancellationToken.None);

                if (!result)
                {
                    throw new Exception("Server failed to stop");
                }

                _logger.LogInformation($"Server stopped. ServerID: {server.ID}");

                server.ShouldBeRunning = false;

                _context.Update(server);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"Error occurred when stopping server. ServerID: {server.ID}");

                return false;
            }
        }

        public async Task<bool> DestroyContainer(Server server)
        {
            if (string.IsNullOrEmpty(server.ContainerID))
            {
                _logger.LogWarning($"Tried to destroy container when no container id was set for server. ServerID: {server.ID}");

                return true;
            }

            try
            {

                _logger.LogInformation($"About to destroy container for server. ServerID: {server.ID} ContainerID: {server.ContainerID}");

                var container = await DockerQueryHelper.GetContainer(_dockerClient, server.ContainerID);
                if (container.Status == "running")
                {
                    await Stop(server);
                }

                await _dockerClient.Containers.RemoveContainerAsync(server.ContainerID, new Docker.DotNet.Models.ContainerRemoveParameters
                {
                    Force = true
                });

                server.ContainerID = null;

                _context.Update(server);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"Error occurred when removing container for server. ServerID: {server.ID}");

                return false;
            }
        }

        private async Task CreateContainer(Server server)
        {
            _logger.LogInformation($"About to create container for server. ServerID: {server.ID}, ImageID: {server.Image.ID}");

            var worldPath = Path.Combine(_environment.ContentRootPath, _configuration.WorldDirectory, server.World.Path);
            var serverPropertiesPath = Path.Combine(_environment.ContentRootPath, _configuration.ServerPropertiesDirectory, $"{server.ID}-server.properties");

            var heapMax = server.MemoryAllocationMB;
            var heapStart = Convert.ToInt32(server.MemoryAllocationMB * 0.6);

            var javaOpts = $"JAVA_OPTS=-Xmx{heapMax}m -Xms{heapStart}m";

            File.WriteAllText(serverPropertiesPath, server.SerializedProperties);

            var mods = server.Image.SupportsMods ? server.Mods.Select(m => Path.Combine(_configuration.ModDirectory, m.Path))
                                                 : null;

            var response = await _dockerClient.Containers.CreateContainerAsync(new Docker.DotNet.Models.CreateContainerParameters
            {
                Image = server.Image.DockerId,
                Env = new List<string> { javaOpts },
                ExposedPorts = new Dictionary<string, object>()
                {
                    { server.MainPort.ToString() + "/tcp", new { } },
                    { server.QueryPort.ToString() + "/tcp", new { }},
                    { server.RconPort.ToString() + "/tcp", new { }}
                },
                //Volumes = new Dictionary<string, object>()
                //{
                //    { "/server/world", new { } },
                //    { "/server/server.properties", new { } }
                //},
                Labels = new Dictionary<string, string>()
                {
                    { "creator", "mineman" }
                },
                HostConfig = new Docker.DotNet.Models.HostConfig
                {
                    Binds = new List<string>
                    {
                        $"{worldPath}:/server/world",
                        $"{serverPropertiesPath}:/server/server.properties"
                    }
                }
            });

            server.ContainerID = response.ID;

            _context.Update(server);
            await _context.SaveChangesAsync();
        }
    }
}
