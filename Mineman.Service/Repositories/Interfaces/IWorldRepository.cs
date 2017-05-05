using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using Mineman.Common.Database.Models;

namespace Mineman.Service.Repositories
{
    public interface IWorldRepository
    {
        Task AddEmpty(string displayName);
        Task AddFromZip(string displayName, ZipArchive zipArchive);
        ICollection<World> Get();
    }
}