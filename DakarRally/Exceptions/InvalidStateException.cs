using System;

namespace DakarRally.Exceptions
{
    /// <summary>
    /// InvalidStateException class.
    /// </summary>
    public class InvalidStateException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStateException"/> class.
        /// </summary>
        public InvalidStateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStateException"/> class.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public InvalidStateException(string message)
            : base(message)
        {
        }
    }
}