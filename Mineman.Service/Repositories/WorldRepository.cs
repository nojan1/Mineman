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

namespace Mineman.Service.Repositories
{
    public class WorldRepository : IWorldRepository
    {
        private readonly DatabaseContext _context;
        private readonly Configuration _configuration;
        private readonly IHostingEnvironment _environment;

        public WorldRepository(DatabaseContext context,
                               IOptions<Configuration> configuration,
                               IHostingEnvironment environment)
        {
            _context = context;
            _configuration = configuration.Value;
            _environment = environment;
        }

        public ICollection<World> Get()
        {
            return _context.Worlds.ToList();
        }

        public async Task<MapImagePaths> GetMapImages(int worldId)
        {
            var world = await _context.Worlds.FindAsync(worldId);

            var fullSizePath = _environment.BuildPath(_configuration.WorldDirectory, world.Path, "map.png");
            var thumbPath = _environment.BuildPath(_configuration.WorldDirectory, world.Path, "map_thumb.png");

            return new MapImagePaths
            {
                MapPath = File.Exists(fullSizePath) ? fullSizePath : null,
                MapThumbPath = File.Exists(thumbPath) ? thumbPath : null
            };
        }

        public async Task<World> AddEmpty(string displayName)
        {
            return await AddFromZip(displayName, null);
        }

        public async Task<World> AddFromZip(string displayName, ZipArchive zipArchive)
        {
            var folderName = Guid.NewGuid().ToString("N");
            var worldFolderPath = Path.Combine(_environment.ContentRootPath, _configuration.WorldDirectory, folderName);

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
    }
}
