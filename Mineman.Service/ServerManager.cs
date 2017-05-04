﻿using Docker.DotNet;
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

namespace Mineman.Service
{
    public class ServerManager
    {
        private readonly DatabaseContext _context;
        private readonly IDockerClient _dockerClient;
        private readonly Configuration _configuration;

        public ServerManager(DatabaseContext context,
                             IDockerClient dockerClient,
                             Configuration configuration)
        {
            _context = context;
            _dockerClient = dockerClient;
            _configuration = configuration;
        }

        public async Task Start(Server server)
        {
            if(string.IsNullOrEmpty(server.ContainerID) || 
               await DockerQueryHelper.GetContainer(_dockerClient, server.ContainerID) != null)
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
        }

        public async Task Stop(Server server)
        {
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
        }

        private async Task CreateContainer(Server server)
        {
            var worldPath = Path.Combine(_configuration.WorldDirectory, server.World.Path);
            var serverPropertiesPath = Path.Combine(_configuration.ServerPropertiesDirectory, $"{server.ID}-server.properties");

            var heapMax = server.MemoryAllocationMB;
            var heapStart = server.MemoryAllocationMB * 0.6;

            var javaOpts = $"JAVA_OPTS='-Xmx{heapMax}m -Xms{heapStart}m'";

            File.WriteAllText(serverPropertiesPath, server.SerializedProperties);

            var mods = server.Image.SupportsMods ? server.Mods.Select(m => Path.Combine(_configuration.ModDirectory, m.Path))
                                                 : null;

            var response = await _dockerClient.Containers.CreateContainerAsync(new Docker.DotNet.Models.CreateContainerParameters
            {
                Image = server.Image.Tag,
                Env = new List<string> { javaOpts },
                ExposedPorts = new Dictionary<string, object>()
                {
                    { server.MainPort.ToString() + "/tcp", true},
                    { server.QueryPort.ToString() + "/tcp", true},
                    { server.RconPort.ToString() + "/tcp", true}
                },
                Volumes = new Dictionary<string, object>()
                {

                },
                Labels = new Dictionary<string, string>()
                {
                    { "creator", "mineman" }
                }
            });

            server.ContainerID = response.ID;

            //TODO: Save container ID to database
        }
    }
}
