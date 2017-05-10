using System.Threading.Tasks;
using Mineman.Common.Database.Models;

namespace Mineman.Service.MinecraftQuery
{
    public interface IMinecraftServerQuery
    {
        Task<QueryInformation> GetInfo(Server server);
    }
}