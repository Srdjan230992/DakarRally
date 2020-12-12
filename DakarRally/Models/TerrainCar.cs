namespace DakarRally.Models
{
    public sealed class TerrainCar : Car
    {
        public TerrainCar() : base()
        {
            VehicleSpeed = 100;// base.CalculateVehicleSpeed(100);
            LightMalfunctionProbability = 0.03;
            HeavyMalfunctionProbability = 0.01;
        }
    }
}
