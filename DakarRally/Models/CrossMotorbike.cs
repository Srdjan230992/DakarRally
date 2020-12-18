namespace DakarRally.Models
{
    /// <summary>
    /// CrossMotorbike class.
    /// </summary>
    public sealed class CrossMotorbike : Motorbike
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrossMotorbike"/> class.
        /// </summary>
        public CrossMotorbike() : base()
        {
            VehicleSpeed = 85;
            LightMalfunctionProbability = 0.03;
            HeavyMalfunctionProbability = 0.02;
        }
    }
}