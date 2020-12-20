using DakarRally.Models;
using System.Threading.Tasks;

namespace DakarRally.Repository
{
    /// <summary>
    /// DakarRallyRepository class.
    /// </summary>
    public class DakarRallyRepository : IDakarRally
    {
        #region Fields

        private VehicleDbContext _dbContext;
        private BaseRepository<Vehicle> _vehicles;
        private BaseRepository<Race> _races;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DakarRallyRepository"/> class.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        public DakarRallyRepository(VehicleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Methods and Properties

        /// <inheritdoc/>
        public IRepository<Vehicle> Vehicles
        {
            get
            {
                return _vehicles ??
                    (_vehicles = new BaseRepository<Vehicle>(_dbContext));
            }
        }

        /// <inheritdoc/>
        public IRepository<Race> Races
        {
            get
            {
                return _races ??
                    (_races = new BaseRepository<Race>(_dbContext));
            }
        }

        /// <inheritdoc/>
        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        /// <inheritdoc/>
        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}