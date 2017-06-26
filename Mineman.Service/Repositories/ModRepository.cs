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
using Microsoft.EntityFrameworkCore;
using Mineman.Common.Models.Configuration;

namespace Mineman.Service.Repositories
{
    public class ModInUseException : Exception { }

    public class ModRepository : IModRepository
    {
        private readonly IHostingEnvironment _environment;
        private readonly PathOptions _pathOptions;
        private readonly DatabaseContext _context;

        public ModRepository(DatabaseContext context,
                             IHostingEnvironment environment,
                             IOptions<PathOptions> pathOptions)
        {
            _context = context;
            _environment = environment;
            _pathOptions = pathOptions.Value;
        }

        public ICollection<Mod> GetMods()
        {
            var mods = _context.Mods.ToList();
            return mods;
        }

        public async Task<Mod> Get(int modId)
        {
            return await _context.Mods.FindAsync(modId);
        }

        public async Task<Mod> Add(ModAddModel modAddModel)
        {
            string fileName = modAddModel.ModFile.First().FileName;
            string modFilePath = _environment.BuildPath(_pathOptions.ModDirectory, fileName);

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

        public async Task Delete(int modId)
        {
            var modInUse = await _context.Servers.Include(s => s.Mods)
                                                   .SelectMany(s => s.Mods)
                                                   .Select(m => m.ID)
                                                   .ContainsAsync(modId);

            if (modInUse)
            {
                throw new ModInUseException();
            }

            var mod = await _context.Mods.FindAsync(modId);
            _context.Mods.Remove(mod);

            await _context.SaveChangesAsync();
        }

        public IDictionary<int, Server[]> GetModUsage()
        {
            var servers = _context.Servers.Include(s => s.Mods).ToList();
            var mods = _context.Mods.ToList();

            return mods.ToDictionary(m => m.ID,
                m => servers.Where(s => s.Mods.Any(m2 => m2.ID == m.ID)).ToArray());

        }
    }
}
