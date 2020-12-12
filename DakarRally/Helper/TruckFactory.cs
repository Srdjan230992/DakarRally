using DakarRally.Models;

namespace DakarRally.Helper
{
    public class TruckFactory : VehicleFactory
    {
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<Truck>(jsonResult);
        }
    }
}
