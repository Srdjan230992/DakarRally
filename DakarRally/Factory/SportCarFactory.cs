using DakarRally.Models;

namespace DakarRally.Factory
{
    /// <summary>
    /// SportCarFactory class.
    /// </summary>
    public class SportCarFactory : VehicleFactory
    {
        /// <inheritdoc/>
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<SportCar>(jsonResult);
        }
    }
}
