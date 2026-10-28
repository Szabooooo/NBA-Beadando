using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;

namespace NBADatabase.Data
{
    public class NBADbContext : DbContext
    {
        public NBADbContext(DbContextOptions<NBADbContext> options) : base(options)
        {
         
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasOne(g => g.HomeTeam)
                .WithMany()
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.AwayTeam)
                .WithMany()
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Game> Games => Set<Game>();
        public DbSet<Team> Team => Set<Team>();
        public DbSet<Player> Players => Set<Player>();
    }
}
