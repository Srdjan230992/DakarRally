namespace DakarRally.Models
{
    /// <summary>
    /// Car class.
    /// </summary>
    public class Car : Vehicle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Car"/> class.
        /// </summary>
        public Car()
        {
            LightMalfunctionDelay = 5;
        }
    }
}