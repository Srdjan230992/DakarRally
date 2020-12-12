namespace DakarRally.Helper
{
    public static class AppHelper
    {
        public enum RaceState
        {
            Pending,
            Running,
            Finished
        }

        public enum VehicleStatus
        {
            NoStatus,
            FinishRace,
            BreakDown
        }

        public enum LogicOperator
        {
            AND,
            OR
        }
    }
}
