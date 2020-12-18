using System;

namespace DakarRally.Exceptions
{
    /// <summary>
    /// VehiclesNotFoundException class.
    /// </summary>
    public class VehiclesNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesNotFoundException"/> class.
        /// </summary>
        public VehiclesNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public VehiclesNotFoundException(string message)
            : base(message)
        {
        }
    }
}