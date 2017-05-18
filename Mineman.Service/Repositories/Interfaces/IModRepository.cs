using System.Collections.Generic;
using System.Threading.Tasks;
using Mineman.Common.Database.Models;

namespace Mineman.Service.Repositories
{
    public interface IModRepository
    {
        Task<Mod> Get(int modId);
        ICollection<Mod> GetMods();
    }
}