namespace DakarRally.Models
{
    /// <summary>
    /// Motorbike class.
    /// </summary>
    public class Motorbike : Vehicle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Motorbike"/> class.
        /// </summary>
        public Motorbike()
        {
            LightMalfunctionDelay = 3;
        }
    }
}
