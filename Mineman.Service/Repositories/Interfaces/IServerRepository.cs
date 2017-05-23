using System.Collections.Generic;
using System.Threading.Tasks;
using Mineman.Common.Models;
using Mineman.Common.Models.Client;
using Mineman.Common.Database.Models;

namespace Mineman.Service.Repositories
{
    public interface IServerRepository
    {
        Task<Server> Add(ServerAddModel serverAddModel);
        Task<ICollection<ServerWithDockerInfo>> GetServers();
        Task<ServerWithDockerInfo> GetWithDockerInfo(int id);
        Task<Server> Get(int id);
        Task<Server> UpdateConfiguration(int id, ServerConfigurationModel configurationModel);
    }
}