using DakarRally.Models;

namespace DakarRally.Helper
{
    public class SportCarFactory : VehicleFactory
    {
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<SportCar>(jsonResult);
        }
    }
}
