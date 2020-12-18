using System;

namespace DakarRally.Exceptions
{
    /// <summary>
    /// RacesNotFoundException class.
    /// </summary>
    public class RacesNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RacesNotFoundException"/> class.
        /// </summary>
        public RacesNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RacesNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public RacesNotFoundException(string message)
            : base(message)
        {
        }
    }
}