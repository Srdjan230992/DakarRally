using Microsoft.EntityFrameworkCore;

namespace DakarRally.Models
{
    public class VehicleDbContext : DbContext
    {
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options)
            : base(options)
        {
        }
        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Race> Races { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SportCar>();
            builder.Entity<TerrainCar>();
            builder.Entity<CrossMotorbike>();
            builder.Entity<SportMotorbike>();
            builder.Entity<Truck>();

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
