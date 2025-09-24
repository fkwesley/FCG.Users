using FCG.Domain.Entities;
using FCG.FiapCloudGames.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Context
{
    public class UsersDbContext : DbContext
    {
        private readonly string _connectionString;

        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<Trace> Traces { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

            // Alternatively, you can apply configurations explicitly if needed
            //modelBuilder.ApplyConfiguration(new GameConfiguration());
            //modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
