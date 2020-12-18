namespace DakarRally.Models
{
    /// <summary>
    /// SportMotorbike class.
    /// </summary>
    public sealed class SportMotorbike : Motorbike
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SportMotorbike"/> class.
        /// </summary>
        public SportMotorbike() : base()
        {
            VehicleSpeed = 130;
            LightMalfunctionDelay = 3;
            LightMalfunctionProbability = 0.18;
            HeavyMalfunctionProbability = 0.1;
        }
    }
}