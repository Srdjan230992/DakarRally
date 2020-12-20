using DakarRally.Models;
using System.Threading.Tasks;

namespace DakarRally.Services
{
    /// <summary>
    /// Interface for race manipulation.
    /// </summary>
    public interface IRaceService
    {
        /// <summary>
        /// Creates a new race.
        /// </summary>
        /// <param name="year">Race year (id).</param>
        /// <returns>Created race.</returns>
        public Task<Race> CreateRace(int year);

        /// <summary>
        /// Add vehicle to the race.
        /// </summary>
        /// <param name="vehicle">The vehicle that will be added.</param>
        /// <param name="raceId">The race id.</param>
        /// <returns>Created vehicle.</returns>
        public Task<Vehicle> AddVehicleToRace(Vehicle vehicle, int raceId);

        /// <summary>
        /// Update vehicle informations.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <param name="id">Vehicle id.</param>
        public Task UpdateVehicleInfo(Vehicle vehicle, long id);

        /// <summary>
        /// Delete vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        public Task DeleteVehicle(long id);

        /// <summary>
        /// Starts the race.
        /// </summary>
        /// <param name="raceID">Vehicle id.</param>
        /// <returns>The race.</returns>
        public Race StartRace(int raceID);
    }
}