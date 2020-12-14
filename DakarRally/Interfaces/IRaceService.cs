using DakarRally.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRally.Interfaces
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
        /// <returns>Race creation result.</returns>
        public Task<int> CreateRace(Race race);

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
        /// <returns>Vehicle adding result.</returns>
        public Task<int> AddVehicleToRace(Vehicle vehicle);

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
        /// <returns>Vehicle update result.</returns>
        public Task<int> UpdateVehicleInfo(Vehicle vehicle);

        /// <summary>
        /// Delete vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Vehicle delete result.</returns>
        public Task<int> DeleteVehicle(long id);

        /// <summary>
        /// Starts the race.
        /// </summary>
        public void StartRace(int raceID);

        /// <summary>
        /// Checks if any race is in running state.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfAnyRaceIsRunning();

        /// <summary>
        /// 
        /// </summary>
        public void PopulateInitData();
    }
}
