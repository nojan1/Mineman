using System.Threading.Tasks;
using Mineman.Common.Database.Models;

namespace Mineman.Service.Repositories
{
    public interface IPlayerRepository
    {
        Task<PlayerProfile> Get(string uuid);
    }
}