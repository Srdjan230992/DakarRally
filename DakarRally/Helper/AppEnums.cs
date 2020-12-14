namespace DakarRally.Helper
{
    /// <summary>
    /// Application enumerations class.
    /// </summary>
    public static class AppEnums
    {
        /// <summary>
        /// Race status.
        /// </summary>
        public enum RaceState
        {
            Pending,
            Running,
            Finished
        }

        /// <summary>
        /// Vehicle status.
        /// </summary>
        public enum VehicleStatus
        {
            NoStatus,
            FinishRace,
            BreakDown
        }
    }
}
