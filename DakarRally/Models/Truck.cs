namespace DakarRally.Models
{
    /// <summary>
    /// Truck class.
    /// </summary>
    public sealed class Truck : Vehicle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Truck"/> class.
        /// </summary>
        public Truck()
        {
            VehicleSpeed = 80;
            LightMalfunctionDelay = 7;
            LightMalfunctionProbability = 0.06;
            HeavyMalfunctionProbability = 0.04;
        }
    }
}