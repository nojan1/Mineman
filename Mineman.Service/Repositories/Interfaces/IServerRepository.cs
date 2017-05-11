using System.Collections.Generic;
using System.Threading.Tasks;
using Mineman.Common.Models;
using Mineman.Common.Models.Client;
using Mineman.Common.Database.Models;

namespace Mineman.Service.Repositories
{
    public interface IServerRepository
    {
        Task Add(ServerAddModel serverAddModel);
        Task<ICollection<ServerWithDockerInfo>> GetServers();
        Task<Server> Get(int id);
        Task<Server> UpdateConfiguration(int id, ServerConfigurationModel configurationModel);
    }
}