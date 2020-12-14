namespace DakarRally.Models
{
    /// <summary>
    /// SportCar class.
    /// </summary>
    public sealed class SportCar : Car
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SportCar"/> class.
        /// </summary>
        public SportCar() : base()
        {
            VehicleSpeed = 140;
            LightMalfunctionProbability = 0.12;
            HeavyMalfunctionProbability = 0.02;
        }
    }
}
