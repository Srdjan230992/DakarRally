using System.Collections.Generic;
using static DakarRally.Helper.AppEnums;

namespace DakarRally.Helper
{
    /// <summary>
    /// RaceStatus class.
    /// </summary>
    public class RaceStatus
    {
        /// <summary>
        /// Current race status.
        /// </summary>
        public string Status { get; set; } = RaceState.Pending.ToString();

        /// <summary>
        /// Vehicle status to number of vehicles mapping.
        /// </summary>
        public Dictionary<string, int> VehicleStatusToNumberOfVehicles { get; set; } = new Dictionary<string, int>(1);

        /// <summary>
        /// Vehicle type to number of vehicles mapping.
        /// </summary>
        public Dictionary<string, int> VehicleTypeToNumberOfVehicles { get; set; } = new Dictionary<string, int>(1);
    }
}
