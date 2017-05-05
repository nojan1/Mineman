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

        public async Task AddEmpty(string displayName)
        {
            await AddFromZip(displayName, null);
        }

        public async Task AddFromZip(string displayName, ZipArchive zipArchive)
        {
            var folderName = Guid.NewGuid().ToString("N");
            var worldFolderPath = Path.Combine(_environment.ContentRootPath, _configuration.WorldDirectory, folderName);

            Directory.CreateDirectory(worldFolderPath);

            if(zipArchive != null)
            {
                zipArchive.ExtractToDirectory(worldFolderPath);
            }

            await _context.Worlds.AddAsync(new World
            {
               DisplayName = displayName,
               Path = folderName
            });

            await _context.SaveChangesAsync();
        }
    }
}
