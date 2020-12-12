namespace DakarRally.Models
{
    public sealed class Truck : Vehicle
    {
        public Truck()
        {
            VehicleSpeed = 80;//base.CalculateVehicleSpeed(80);
            LightMalfunctionDelay = 7;
            LightMalfunctionProbability = 0.06;
            HeavyMalfunctionProbability = 0.04;
        }
    }
}
