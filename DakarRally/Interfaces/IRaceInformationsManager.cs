using DakarRally.Helper;
using DakarRally.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DakarRally.Interfaces
{
    public interface IRaceInformationsManager
    {
        public Task<List<Vehicle>> GetLeaderboardForAllVehicles();
        public Task<List<Vehicle>> GetLeaderboardForVehicle(string type);
        public VehicleStatistic GetVehicleStatistics(long vehicleId);
        public RaceStatus GetRaceStatus(int raceId);
        public Task<FilterOutputModel> FindVehicles(FilterData filterData, string order);
    }
}
