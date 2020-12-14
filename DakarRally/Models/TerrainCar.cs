namespace DakarRally.Models
{
    /// <summary>
    /// TerrainCar class.
    /// </summary>
    public sealed class TerrainCar : Car
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainCar"/> class.
        /// </summary>
        public TerrainCar() : base()
        {
            VehicleSpeed = 100;
            LightMalfunctionProbability = 0.03;
            HeavyMalfunctionProbability = 0.01;
        }
    }
}
