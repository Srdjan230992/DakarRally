using DakarRally.Models;

namespace DakarRally.Factory
{
    /// <summary>
    /// SportMotorbikeFactory class.
    /// </summary>
    public class SportMotorbikeFactory : VehicleFactory
    {
        /// <inheritdoc/>
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<SportMotorbike>(jsonResult);
        }
    }
}
