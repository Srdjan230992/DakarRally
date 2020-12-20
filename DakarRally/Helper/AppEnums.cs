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
            PENDING,
            RUNNING,
            FINISHED
        }

        /// <summary>
        /// Vehicle status.
        /// </summary>
        public enum VehicleStatus
        {
            NOSTATUS,
            FINISHRACE,
            BREAKDOWN
        }

        /// <summary>
        /// Vehicle type.
        /// </summary>
        public enum VehicleType
        {
            SPORTCAR,
            TERRAINCAR,
            SPORTMOTORBIKE,
            CROSSMOTORBIKE,
            TRUCK
        }

        /// <summary>
        /// Vehicles type.
        /// </summary>
        public enum VehiclesType
        {
            CARS,
            TRUCKS,
            MOTORCYCLES
        }

        /// <summary>
        /// Order type.
        /// </summary>
        public enum OrderType
        {
            ASC,
            DESC
        }
    }
}