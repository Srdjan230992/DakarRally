using DakarRally.Models;
using System.Collections.Generic;

namespace DakarRally.Controllers
{
    /// <summary>
    /// FilteredVehiclesResponse class.
    /// </summary>
    public class FilteredVehiclesResponse
    {
        /// <summary>
        /// Number of filter results.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Filter result items.
        /// </summary>
        public IEnumerable<Vehicle> Items { get; set; }
    }
}