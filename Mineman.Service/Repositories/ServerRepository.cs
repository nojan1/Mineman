using Docker.DotNet;
using Mineman.Common.Database;
using Mineman.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Docker.DotNet.Models;
using Mineman.Common.Database.Models;
using Mineman.Common.Models.Client;
using Mineman.Service.Helpers;

namespace Mineman.Service.Repositories
{
    public class ServerRepository : IServerRepository
    {
        private readonly DatabaseContext _context;
        private readonly IDockerClient _dockerClient;

        public ServerRepository(DatabaseContext context,
                                IDockerClient dockerClient)
        {
            _context = context;
            _dockerClient = dockerClient;
        }

        public async Task Add(ServerAddModel serverAddModel)
        {
            var server = new Server
            {
                Description = serverAddModel.Description,
                Image = _context.Images.FirstOrDefault(i => i.ID == serverAddModel.ImageID),
                World = _context.Worlds.FirstOrDefault(w => w.ID == serverAddModel.WorldID)
            };

            if(server.Image.SupportsMods && serverAddModel.ModIDs != null)
            {

                server.Mods = serverAddModel.ModIDs.Select(id => _context.Mods.FirstOrDefault(m => m.ID == id)).ToArray();
            }

            await _context.Servers.AddAsync(server);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<ServerWithDockerInfo>> GetServers()
        {
            var serversDb = _context.Servers.ToList();
            var containerInfo = await DockerQueryHelper.GetContainers(_dockerClient);

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
