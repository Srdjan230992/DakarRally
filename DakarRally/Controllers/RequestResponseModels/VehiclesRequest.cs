using DakarRally.Helper;

namespace DakarRally.Controllers
{
    /// <summary>
    /// VehiclesRequest class.
    /// </summary>
    public class VehiclesRequest
    {
        /// <summary>
        /// Team name.
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Vehicle model.
        /// </summary>
        public FilterItem Model { get; set; }

        /// <summary>
        /// Vehicle manufacturing date.
        /// </summary>
        public FilterItem ManufacturingDate { get; set; }

        /// <summary>
        /// Vehicle status.
        /// </summary>
        public FilterItem Status { get; set; }

        /// <summary>
        /// Vehicle distance.
        /// </summary>
        public FilterItem Distance { get; set; }
    }
}