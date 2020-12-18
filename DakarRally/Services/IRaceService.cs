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
        /// <param name="race">Race to create.</param>
        public Task CreateRace(Race race);

        /// <summary>
        /// Find race by race id.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        /// <returns>Race with specified id.</returns>
        public Task<Race> FindRaceById(long raceId);

        /// <summary>
        /// Add vehicle to the race.
        /// </summary>
        /// <param name="vehicle">The vehicle that will be added.</param>
        public Task AddVehicleToRace(Vehicle vehicle);

        /// <summary>
        /// Find vehicle by vehicle id.
        /// </summary>
        /// <param name="vehicleId">Vehicle id.</param>
        /// <returns>Vehicle for specified id.</returns>
        public Task<Vehicle> FindVehicleById(long vehicleId);

        /// <summary>
        /// Update vehicle informations.
        /// </summary>
        /// <param name="vehicle">Vehicle id.</param>
        public Task UpdateVehicleInfo(Vehicle vehicle);

        /// <summary>
        /// Delete vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        public Task DeleteVehicle(long id);

        /// <summary>
        /// Starts the race.
        /// </summary>
        public void StartRace(int raceID);
    }
}
