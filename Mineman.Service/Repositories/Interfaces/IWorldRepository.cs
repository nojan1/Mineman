using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using Mineman.Common.Database.Models;
using Mineman.Service.Models;

namespace Mineman.Service.Repositories
{
    public interface IWorldRepository
    {
        Task<World> AddEmpty(string displayName);
        Task<World> AddFromZip(string displayName, ZipArchive zipArchive);
        ICollection<World> Get();
        Task<MapImagePaths> GetMapImagePaths(int worldId);
        Task<string> GetWorldInfoPath(int worldId);
        Task<string> GetWorldMapInfoPath(int worldId);
    }
}