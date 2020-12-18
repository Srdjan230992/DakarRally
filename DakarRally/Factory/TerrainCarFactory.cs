using DakarRally.Models;

namespace DakarRally.Factory
{
    /// <summary>
    /// TerrainCarFactory class.
    /// </summary>
    public class TerrainCarFactory : VehicleFactory
    {
        /// <inheritdoc/>
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<TerrainCar>(jsonResult);
        }
    }
}
