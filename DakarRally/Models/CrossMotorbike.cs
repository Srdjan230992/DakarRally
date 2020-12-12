namespace DakarRally.Models
{
    public sealed class CrossMotorbike : Motorbike
    {
        public CrossMotorbike() : base()
        {
            VehicleSpeed = 85;// base.CalculateVehicleSpeed(85);
            LightMalfunctionProbability = 0.03;
            HeavyMalfunctionProbability = 0.02;
        }
    }
}
