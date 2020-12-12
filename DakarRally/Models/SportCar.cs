namespace DakarRally.Models
{
    public sealed class SportCar : Car
    {
        public SportCar() : base()
        {
            VehicleSpeed = 140;//base.CalculateVehicleSpeed(140);
            LightMalfunctionProbability = 0.12;
            HeavyMalfunctionProbability = 0.02;
        }
    }
}
