using DakarRally.Models;
using System.Linq;
using System.Threading.Tasks;
using static DakarRally.Helper.AppHelper;

namespace DakarRally.Interfaces
{
    public interface IRaceManager
    {
        public Task<int> CreateRace(Race race);
        public Task<Race> FindRaceById(long raceId);
        public Task<int> AddVehicleToRace(Vehicle vehicle);
        public Task<Vehicle> FindVehicleById(long vehicleId);
        public Task<int> UpdateVehicleInfo(Vehicle vehicle);
        public Task<int> DeleteVehicle(long id);
        public bool CheckIfAnyRaceIsRunning();
        public void StartRace(int raceId);
        public IQueryable<Vehicle> FindVehiclesWithRaceId(int raceId);
        public void PopulateInitData();
    }
}
