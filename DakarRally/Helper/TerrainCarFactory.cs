using DakarRally.Models;

namespace DakarRally.Helper
{
    public class TerrainCarFactory : VehicleFactory
    {
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<TerrainCar>(jsonResult);
        }
    }
}
