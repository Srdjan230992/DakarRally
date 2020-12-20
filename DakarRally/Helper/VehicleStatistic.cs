namespace DakarRally.Helper
{
    /// <summary>
    /// VehicleStatistic class;
    /// </summary>
    public class VehicleStatistic
    {
        #region Fields

        private int _distance;
        private bool _isHeayMalfanctionOssured;
        private int _lightMulfunctionCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleStatistic"/> class.
        /// </summary>
        /// <param name="distance">Distance.</param>
        /// <param name="isHeayMalfanctionOssured">Is heavy manufacturing occured.</param>
        /// <param name="lightMulfunctionCount">Light manufacturing count.</param>
        public VehicleStatistic(int distance, bool isHeayMalfanctionOssured, int lightMulfunctionCount)
        {
            _distance = distance;
            _isHeayMalfanctionOssured = isHeayMalfanctionOssured;
            _lightMulfunctionCount = lightMulfunctionCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Distance.
        /// </summary>
        public int  Distance { get { return _distance; } set { _distance = value; } }

        /// <summary>
        /// Is heavy manufacturing occured.
        /// </summary>
        public bool IsHeayMalfanctionOssured { get { return _isHeayMalfanctionOssured; } set { _isHeayMalfanctionOssured = value; } }

        /// <summary>
        /// Light manufacturing count.
        /// </summary>
        public int LightMulfunctionCount { get { return _lightMulfunctionCount; } set { _lightMulfunctionCount = value; } }

        #endregion
    }
}