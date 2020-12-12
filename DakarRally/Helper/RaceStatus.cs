using System.Collections.Generic;
using static DakarRally.Helper.AppHelper;

namespace DakarRally.Helper
{
    public class RaceStatus
    {
        public string Status { get; set; } = RaceState.Pending.ToString();
        public Dictionary<string, int> VehicleStatusToNumberOfVehicles { get; set; } = new Dictionary<string, int>(1);
        public Dictionary<string, int> VehicleTypeToNumberOfVehicles { get; set; } = new Dictionary<string, int>(1);
    }
}
