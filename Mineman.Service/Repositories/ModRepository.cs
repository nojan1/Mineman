using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Repositories
{
    public class ModRepository : IModRepository
    {
        private readonly DatabaseContext _context;

        public ModRepository(DatabaseContext context)
        {
            _context = context;
        }

        public ICollection<Mod> GetMods()
        {
            return _context.Mods.ToList();
        }

        public async Task<Mod> Get(int modId)
        {
            return await _context.Mods.FindAsync(modId);
        } 
    }
}
