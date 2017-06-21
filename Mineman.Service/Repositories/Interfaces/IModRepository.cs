using System.Collections.Generic;
using System.Threading.Tasks;
using Mineman.Common.Database.Models;
using Mineman.Common.Models.Client;

namespace Mineman.Service.Repositories
{
    public interface IModRepository
    {
        Task<Mod> Get(int modId);
        ICollection<Mod> GetMods();
        Task<Mod> Add(ModAddModel modAddModel);
        Task Delete(int modId);
        IDictionary<int, Server[]> GetModUsage();
    }
}