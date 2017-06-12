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
    public enum ServerStartResult
    {
        Success = 0,
        Fail = 1,
        LaterStart = 2
    }

    public class ServerManager : ResourceLockingManagerBase, IServerManager
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

        public async Task<ServerStartResult> Start(Server server)
        {
            using (ClaimResource(server.ID))
            {
                try
                {
                    _logger.LogInformation($"About to start server. ServerID: {server.ID}");

                    if (server.Image == null)
                    {
                        throw new ArgumentNullException("No image has been set on server");
                    }

                    if (string.IsNullOrEmpty(server.ContainerID) ||
                       await DockerQueryHelper.GetContainer(_dockerClient, server.ContainerID) == null ||
                       server.NeedsRecreate)
                    {
                        _logger.LogInformation($"Needs to create container before starting. ServerID: {server.ID}");

                        if (server.Image.BuildStatus != null && !server.Image.BuildStatus.BuildSucceeded)
                        {
                            throw new Exception($"Unable to start server! Image build did not succeed. ServerID: {server.ID}");
                        }

                        if (string.IsNullOrEmpty(server.Image.DockerId) || server.Image.BuildStatus == null)
                        {
                            _logger.LogInformation($"Unable to create container underlying image not yet built. Marking server for later start. ServerID: {server.ID}");

                            server.ShouldBeRunning = true;

                            _context.Update(server);
                            await _context.SaveChangesAsync();

                            return ServerStartResult.LaterStart;
                        }

                        if (!string.IsNullOrEmpty(server.ContainerID))
                        {
                            await DestroyContainerInternal(server);
                        }

                        await CreateContainer(server);
                    }

                    WriteServerProperties(server);

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

                    return ServerStartResult.Success;
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(), e, $"Error occurred when starting server. ServerID: {server.ID}");

                    return ServerStartResult.Fail;
                }
            }
        }

        public async Task<bool> Stop(Server server)
        {
            using (ClaimResource(server.ID))
            {
                try
                {
                    _logger.LogInformation($"About to stop server. ServerID: {server.ID}");

                    await Stop(server.ContainerID);

                    server.ShouldBeRunning = false;

                    if (server.NeedsRecreate)
                    {
                        await DestroyContainerInternal(server);
                    }

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
        }

        private async Task Stop(string containerID)
        {
            _logger.LogInformation($"About to stop container. ContainerId: {containerID}");

            var result = await _dockerClient.Containers.StopContainerAsync(containerID,
            new ContainerStopParameters
            {
                WaitBeforeKillSeconds = 10
            },
            CancellationToken.None);

            if (!result)
            {
                throw new Exception("Container failed to stop");
            }

            _logger.LogInformation($"Container stopped. ContainerID: {containerID}");
        }

        public async Task<bool> DestroyContainer(Server server)
        {
            using (ClaimResource(server.ID))
            {
                return await DestroyContainerInternal(server);
            }
        }

        private async Task<bool> DestroyContainerInternal(Server server)
        {
            try
            {
                if (string.IsNullOrEmpty(server.ContainerID))
                {
                    _logger.LogWarning($"Tried to destroy container when no container id was set for server. ServerID: {server.ID}");

                    return true;
                }

                await DestroyContainerInternal(server.ContainerID);

                server.ContainerID = null;

                _context.Update(server);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, $"Error occurred when destroying container for server. ServerID: {server.ID}");

                return false;
            }
        }
            
        private async Task DestroyContainerInternal(string containerID)
        {
            _logger.LogInformation($"About to destroy container for server. ContainerID: {containerID}");

            var container = await DockerQueryHelper.GetContainer(_dockerClient, containerID);
            if (container.State == "running")
            {
                await Stop(containerID);
            }

            await _dockerClient.Containers.RemoveContainerAsync(containerID, new ContainerRemoveParameters
            {
                Force = true
            });
        }

        public async Task RemoveUnusedContainers()
        {
            var containerIdsInDatabase = _context.Servers.Select(s => s.ContainerID).ToList();
            var containersInDocker = await DockerQueryHelper.GetContainers(_dockerClient);

            foreach (var container in containersInDocker.Where(c => !containerIdsInDatabase.Any(id => c.ID == id)))
            {
                _logger.LogInformation($"Container found in docker but doesn't exist in database, destroying. ContainerID: {container.ID}");
                await DestroyContainerInternal(container.ID);
            }
        }

        private async Task CreateContainer(Server server)
        {
            _logger.LogInformation($"About to create container for server. ServerID: {server.ID}, ImageID: {server.Image.ID}");

            var worldPath = _environment.BuildPath(_configuration.WorldDirectory, server.World.Path);
            var serverPropertiesPath = _environment.BuildPath(_configuration.ServerPropertiesDirectory, $"{server.ID}-server.properties");

            var heapMax = server.MemoryAllocationMB;
            var heapStart = Convert.ToInt32(server.MemoryAllocationMB * 0.6);

            var javaOpts = $"JAVA_OPTS=-Xmx{heapMax}m -Xms{heapStart}m";

            WriteServerProperties(server);

            var binds = new List<string>
            {
                $"{worldPath}:/server/world",
                $"{serverPropertiesPath}:/server/server.properties"
            };

            if (server.Image.SupportsMods && server.Mods != null)
            {
                foreach (var mod in server.Mods)
                {
                    var containerPath = $"/server/{server.Image.ModDirectory}/{mod.Path}";
                    var localPath = _environment.BuildPath(_configuration.ModDirectory, mod.Path);

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
            server.NeedsRecreate = false;

            _context.Update(server);
            await _context.SaveChangesAsync();
        }

        private void WriteServerProperties(Server server)
        {
            var serverPropertiesPath = _environment.BuildPath(_configuration.ServerPropertiesDirectory, $"{server.ID}-server.properties");
            File.WriteAllText(serverPropertiesPath, server.SerializedProperties);
        }
    }
}
