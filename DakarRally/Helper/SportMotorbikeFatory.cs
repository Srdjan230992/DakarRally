using DakarRally.Models;

namespace DakarRally.Helper
{
    public class SportMotorbikeFactory : VehicleFactory
    {
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<SportMotorbike>(jsonResult);
        }
    }
}
