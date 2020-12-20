using DakarRally.Helper;
using DakarRally.Models;
using System.Collections.Generic;

namespace DakarRally.Services
{
    /// <summary>
    /// Interface that provides race informations.
    /// </summary>
    public interface IRaceInformationsService
    {
        /// <summary>
        /// Retrives leaderboard data for all vehicles.
        /// </summary>
        /// <returns>List of vehicles.</returns>
        public List<Vehicle> GetLeaderboardForAllVehicles();

        /// <summary>
        /// Retrives leaderboard data for vehicles with specified type.
        /// </summary>
        /// <param name="type">Vehicle type.</param>
        /// <returns>List of vehicles.</returns>
        public List<Vehicle> GetLeaderboardForVehicle(string type);

        /// <summary>
        /// Retrives vehicle statistics data.
        /// </summary>
        /// <param name="vehicleId">Vehicle id.</param>
        /// <returns>Vehicle statistics.</returns>
        public VehicleStatistic GetVehicleStatistics(long vehicleId);

        /// <summary>
        /// Retrives race status.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        /// <returns>Race status.</returns>
        public RaceStatus GetRaceStatus(int raceId);

        /// <summary>
        /// Retrives filtered vehicles data.
        /// </summary>
        /// <param name="filterData">Specified filter data.</param>
        /// <param name="order">Order (asc/desc).</param>
        /// <returns>Filter output data.</returns>
        public FilteredVehiclesResponse FindVehicles(VehiclesRequest filterData, string order);
    }
}
