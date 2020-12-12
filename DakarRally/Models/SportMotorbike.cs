namespace DakarRally.Models
{
    public sealed class SportMotorbike : Motorbike
    {
        public SportMotorbike() : base()
        {
            VehicleSpeed = 130;//base.CalculateVehicleSpeed(130);
            LightMalfunctionDelay = 3;
            LightMalfunctionProbability = 0.18;
            HeavyMalfunctionProbability = 0.1;
        }
    }
}
