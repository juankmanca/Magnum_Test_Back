using Microsoft.EntityFrameworkCore;
using RPS_Game.API.Entities;

namespace RPS_Game.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Round> Rounds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Game>().HasKey(p => p.Id);
            modelBuilder.Entity<Round>().HasKey(p => p.Id);
        }

    }
}

