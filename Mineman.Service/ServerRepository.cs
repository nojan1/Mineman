using Docker.DotNet;
using Mineman.Common.Database;
using Mineman.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Docker.DotNet.Models;

namespace Mineman.Service
{
    public class ServerRepository
    {
        private readonly DatabaseContext _context;
        private readonly IDockerClient _dockerClient;

        public ServerRepository(DatabaseContext context,
                                IDockerClient dockerClient)
        {
            _context = context;
            _dockerClient = dockerClient;
        }

        public async Task<ICollection<ServerWithDockerInfo>> GetServers()
        {
            var serversDb = _context.Servers.ToList();
            var containerInfo = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    //{ "status", new Dictionary<string, bool> { { "running", true } }  },
                    { "label", new Dictionary<string, bool> { { "creator=mineman", true } }  }
                }
            });

            return serversDb.Select(s => {
                return new ServerWithDockerInfo
                {
                     Server = s,
                     IsAlive = containerInfo.Any(x => x.ID == s.ContainerID && x.Status == "running")
                };
            }).ToList();
        }
    }
}
