using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mineman.Common.Models.Client;
using Mineman.Service.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Mineman.Common.Models;
using System.IO;

namespace Mineman.Service.Repositories
{
    public class ModRepository : IModRepository
    {
        private readonly IHostingEnvironment _environment;
        private readonly Configuration _configuration;
        private readonly DatabaseContext _context;

        public ModRepository(DatabaseContext context,
                             IHostingEnvironment environment,
                             IOptions<Configuration> configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration.Value;
        }

        public ICollection<Mod> GetMods()
        {
            return _context.Mods.ToList();
        }

        public async Task<Mod> Get(int modId)
        {
            return await _context.Mods.FindAsync(modId);
        }

        public async Task<Mod> Add(ModAddModel modAddModel)
        {
            string fileName = modAddModel.ModFile.First().FileName;
            string modFilePath = _environment.BuildPath(_configuration.ModDirectory, fileName);

            using (var file = File.Create(modFilePath))
            {
                var stream = modAddModel.ModFile.First().OpenReadStream();
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(file);
            }

            var mod = new Mod
            {
                DisplayName = modAddModel.DisplayName,
                Path = fileName
            };

            await _context.Mods.AddAsync(mod);
            await _context.SaveChangesAsync();

            return mod;
        }
    }
}
