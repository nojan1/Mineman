using Docker.DotNet;
using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mineman.Service
{
    public class ServerManager
    {
        private readonly DatabaseContext _context;
        private readonly IDockerClient _dockerClient;

        public ServerManager(DatabaseContext context,
                             IDockerClient dockerClient)
        {
            _context = context;
            _dockerClient = dockerClient;
        }

        public async Task Start(Server server)
        {
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
    }
}
