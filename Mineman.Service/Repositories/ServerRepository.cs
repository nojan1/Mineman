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
using Microsoft.EntityFrameworkCore;

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

        public async Task<Server> Add(ServerAddModel serverAddModel)
        {
            var serializedProperties = ServerPropertiesSerializer.Serialize(new ServerProperties());

            var server = new Server
            {
                Description = serverAddModel.Description,
                Image = _context.Images.FirstOrDefault(i => i.ID == serverAddModel.ImageID),
                World = _context.Worlds.FirstOrDefault(w => w.ID == serverAddModel.WorldID),
                SerializedProperties = serializedProperties,
                MainPort = serverAddModel.ServerPort,
                MemoryAllocationMB = serverAddModel.MemoryAllocationMB,
                QueryPort = serverAddModel.ServerPort + 1000,
                RconPort = serverAddModel.ServerPort + 2000
            };

            if(server.Image.SupportsMods && serverAddModel.ModIDs != null)
            {
                server.Mods = serverAddModel.ModIDs.Select(id => _context.Mods.FirstOrDefault(m => m.ID == id)).ToArray();
            }

            await _context.Servers.AddAsync(server);
            await _context.SaveChangesAsync();

            return server;
        }

        public async Task<ICollection<ServerWithDockerInfo>> GetServers()
        {
            var serversDb = _context.Servers.ToList();
            var containerInfo = await DockerQueryHelper.GetContainers(_dockerClient);

            return serversDb.Select(s => {
                return new ServerWithDockerInfo
                {
                     Server = s,
                     IsAlive = containerInfo.Any(x => x.ID == s.ContainerID && x.State == "running")
                };
            }).ToList();
        }

        public async Task<ServerWithDockerInfo> GetWithDockerInfo(int id)
        {
            var server = await Get(id);
            var isAlive = string.IsNullOrEmpty(server.ContainerID) ? false  
                                                                   : (await DockerQueryHelper.GetContainer(_dockerClient, server.ContainerID)).State == "running";

            return new ServerWithDockerInfo
            {
                Server = server,
                IsAlive = isAlive
            };
        }

        public async Task<Server> Get(int id)
        {
            return await _context.Servers.Include(s => s.Image)
                                               .Include(s => s.World)
                                               .Include(s => s.Mods)
                                               .FirstOrDefaultAsync(s => s.ID == id);
        }

        public async Task<Server> UpdateConfiguration(int id, ServerConfigurationModel configurationModel)
        {
            var server = await Get(id);

            var existingProperties = ServerPropertiesSerializer.Deserialize(server.SerializedProperties);
            var mergedProperties = ServerPropertiesSerializer.Merge(existingProperties, configurationModel.Properties);

            server.SerializedProperties = ServerPropertiesSerializer.Serialize(mergedProperties);
            server.MainPort = configurationModel.ServerPort;
            server.MemoryAllocationMB = configurationModel.MemoryAllocationMB;
            server.Description = configurationModel.Description;
            server.World = _context.Worlds.FirstOrDefault(w => w.ID == configurationModel.WorldID);

            if (server.Image.SupportsMods && configurationModel.ModIDs != null)
            {
                server.Mods = configurationModel.ModIDs.Select(i => _context.Mods.FirstOrDefault(m => m.ID == i)).ToArray();
            }

            server.NeedsRecreate = true;

            _context.Update(server);
            await _context.SaveChangesAsync();

            return server;
        }
    }
}
