using Docker.DotNet;
using Docker.DotNet.Models;
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

                var result = await _dockerClient.Containers.StartContainerAsync(server.ContainerID, new ContainerStartParameters
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
                new ContainerStopParameters
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
                if (container.State == "running")
                {
                    await Stop(server);
                }

                await _dockerClient.Containers.RemoveContainerAsync(server.ContainerID, new ContainerRemoveParameters
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

            var binds = new List<string>
            {
                $"{worldPath}:/server/world",
                $"{serverPropertiesPath}:/server/server.properties"
            };

            if (server.Image.SupportsMods && server.Mods != null)
            {
                foreach(var mod in server.Mods) {
                    var containerPath = $"/server/{server.Image.ModDirectory}/{mod.Path}";
                    var localPath = Path.Combine(_configuration.ModDirectory, mod.Path);

                    binds.Append($"{localPath}:{containerPath}");
                }
            }

            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = server.Image.DockerId,
                Env = new List<string> { javaOpts },
                ExposedPorts = new Dictionary<string, object>()
                {
                    { "25565/tcp", new { } },
                    { "26565/udp", new { }},
                    { "27565/tcp", new { }}
                },
                Labels = new Dictionary<string, string>()
                {
                    { "creator", "mineman" }
                },
                HostConfig = new HostConfig
                {
                    Binds = binds,
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {"25565/tcp", new List<PortBinding> { new PortBinding { HostIP = "0.0.0.0", HostPort = server.MainPort.ToString() } } },
                        {"26565/udp", new List<PortBinding> { new PortBinding { HostIP = "0.0.0.0", HostPort = server.QueryPort.ToString() } } },
                        {"27565/tcp", new List<PortBinding> { new PortBinding { HostIP = "0.0.0.0", HostPort = server.RconPort.ToString() } } }
                    }
                }
            });

            server.ContainerID = response.ID;

            _context.Update(server);
            await _context.SaveChangesAsync();
        }
    }
}
