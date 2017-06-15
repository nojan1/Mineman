using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mineman.Common.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Database
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Server> Servers { get; set; }
        public DbSet<World> Worlds { get; set; }
        public DbSet<Mod> Mods { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageBuildStatus> BuildStatuses { get; set; }
        public DbSet<PlayerProfile> PlayerProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
