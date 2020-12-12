namespace DakarRally.Helper
{
    public class VehicleStatistic
    {
        private int _distance;
        private bool _isHeayMalfanctionOssured;
        private int _lightMulfunctionCount;

        public VehicleStatistic(int distance, bool isHeayMalfanctionOssured, int lightMulfunctionCount)
        {
            _distance = distance;
            _isHeayMalfanctionOssured = isHeayMalfanctionOssured;
            _lightMulfunctionCount = lightMulfunctionCount;
        }
        public int  Distance { get { return _distance; } set { _distance = value; } }
        public bool IsHeayMalfanctionOssured { get { return _isHeayMalfanctionOssured; } set { _isHeayMalfanctionOssured = value; } }
        public int LightMulfunctionCount { get { return _lightMulfunctionCount; } set { _lightMulfunctionCount = value; } }
    }
}
