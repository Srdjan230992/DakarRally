using DakarRally.Models;

namespace DakarRally.Factory
{
    /// <summary>
    /// TruckFactory class.
    /// </summary>
    public class TruckFactory : VehicleFactory
    {
        /// <inheritdoc/>
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<Truck>(jsonResult);
        }
    }
}
