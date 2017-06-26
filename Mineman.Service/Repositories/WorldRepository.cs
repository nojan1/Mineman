using System;
using System.Collections.Generic;
using System.Text;
using Mineman.Common.Models.Client;
using Mineman.Common.Database;
using System.IO.Compression;
using System.Threading.Tasks;
using Mineman.Common.Database.Models;
using System.Linq;
using Mineman.Common.Models;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Mineman.Service.Models;
using Mineman.Service.Helpers;
using Microsoft.EntityFrameworkCore;
using Mineman.Common.Models.Configuration;

namespace Mineman.Service.Repositories
{
    public class WorldInUseException : Exception { }

    public class WorldRepository : IWorldRepository
    {
        private readonly DatabaseContext _context;
        private readonly PathOptions _pathOptions;
        private readonly IHostingEnvironment _environment;

        public WorldRepository(DatabaseContext context,
                               IOptions<PathOptions> pathOptions,
                               IHostingEnvironment environment)
        {
            _context = context;
            _pathOptions = pathOptions.Value;
            _environment = environment;
        }

        public ICollection<World> Get()
        {
            return _context.Worlds.ToList();
        }

        public async Task<MapImagePaths> GetMapImagePaths(int worldId)
        {
            var world = await _context.Worlds.FindAsync(worldId);

            var fullSizePath = _environment.BuildPath(_pathOptions.WorldDirectory, world.Path, "map.png");
            var thumbPath = _environment.BuildPath(_pathOptions.WorldDirectory, world.Path, "map_thumb.png");

            return new MapImagePaths
            {
                MapPath = File.Exists(fullSizePath) ? fullSizePath : null,
                MapThumbPath = File.Exists(thumbPath) ? thumbPath : null
            };
        }

        public async Task<string> GetWorldInfoPath(int worldId)
        {
            return await GetPathToWorldFile(worldId, "worldinfo.json");
        }

        public async Task<string> GetWorldMapInfoPath(int worldId)
        {
            return await GetPathToWorldFile(worldId, "render-result.json");
        }

        public async Task<World> AddEmpty(string displayName)
        {
            return await AddFromZip(displayName, null);
        }

        public async Task<World> AddFromZip(string displayName, ZipArchive zipArchive)
        {
            var folderName = Guid.NewGuid().ToString("N");
            var worldFolderPath = _environment.BuildPath(_pathOptions.WorldDirectory, folderName);

            Directory.CreateDirectory(worldFolderPath);

            if(zipArchive != null)
            {
                zipArchive.ExtractToDirectory(worldFolderPath);
            }

            var world = new World
            {
                DisplayName = displayName,
                Path = folderName
            };

            await _context.Worlds.AddAsync(world);
            await _context.SaveChangesAsync();

            return world;
        }

        private async Task<string> GetPathToWorldFile(int worldId, string filename)
        {
            var world = await _context.Worlds.FindAsync(worldId);
            var worldInfoPath = _environment.BuildPath(_pathOptions.WorldDirectory, world.Path, filename);

            return File.Exists(worldInfoPath) ? worldInfoPath : null;
        }

        public IDictionary<int, Server[]> GetWorldUsage()
        {
            var servers = _context.Servers.Include(s => s.World).ToList();
            var worlds = _context.Worlds.ToList();

            return worlds.ToDictionary(i => i.ID,
                i => servers.Where(s => s.World?.ID == i.ID).ToArray());
        }

        public async Task Delete(int worldId)
        {
            var worldInUse = await _context.Servers.Include(s => s.World)
                                                   .Where(s => s.World != null)
                                                   .Select(s => s.World.ID)
                                                   .ContainsAsync(worldId);

            if (worldInUse)
            {
                throw new WorldInUseException();
            }

            var world = await _context.Worlds.FindAsync(worldId);
            _context.Worlds.Remove(world);

            //Remove from filesystem
            var worldFolderPath = _environment.BuildPath(_pathOptions.WorldDirectory, world.Path);
            Directory.Delete(worldFolderPath, true);

            await _context.SaveChangesAsync();
        }
    }
}
