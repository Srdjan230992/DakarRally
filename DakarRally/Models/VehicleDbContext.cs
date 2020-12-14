using Microsoft.EntityFrameworkCore;

namespace DakarRally.Models
{
    /// <summary>
    /// VehicleDbContext class.
    /// </summary>
    public class VehicleDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a DbContext.</param>
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DB set of vehicle entities.
        /// </summary>
        public DbSet<Vehicle> Vehicles { get; set; }

        /// <summary>
        /// DB set of race entities.
        /// </summary>
        public DbSet<Race> Races { get; set; }

        /// <summary>
        /// Customizes the ASP.NET Identity model and overrides the defaults.
        /// </summary>
        /// <param name="builder">Model builder.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SportCar>();
            builder.Entity<TerrainCar>();
            builder.Entity<CrossMotorbike>();
            builder.Entity<SportMotorbike>();
            builder.Entity<Truck>();

            base.OnModelCreating(builder);
        }
    }
}
