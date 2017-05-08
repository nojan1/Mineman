using Microsoft.EntityFrameworkCore;
using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Server> Servers { get; set; }
        public DbSet<World> Worlds { get; set; }
        public DbSet<Mod> Mods { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageBuildStatus> BuildStatuses { get; set; }
    }
}
