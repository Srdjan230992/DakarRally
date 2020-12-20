using System;

namespace DakarRally.Exceptions
{
    /// <summary>
    /// VehicleNotModifiedException class.
    /// </summary>
    public class VehicleNotModifiedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotModifiedException"/> class.
        /// </summary>
        public VehicleNotModifiedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNotModifiedException"/> class.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public VehicleNotModifiedException(string message)
            : base(message)
        {
        }
    }
}