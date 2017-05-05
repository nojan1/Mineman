using System.Collections.Generic;
using System.Threading.Tasks;
using Mineman.Common.Models;
using Mineman.Common.Models.Client;

namespace Mineman.Service.Repositories
{
    public interface IServerRepository
    {
        Task Add(ServerAddModel serverAddModel);
        Task<ICollection<ServerWithDockerInfo>> GetServers();
    }
}