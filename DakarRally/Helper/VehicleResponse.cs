using System;

namespace DakarRally.Helper
{
    /// <summary>
    /// VehicleResponse class;
    /// </summary>
    public class VehicleResponse
    {
        #region Fields

        private string _teamName;
        private string _vehicleModel;
        private DateTime _vehicleManufacturingDate;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleResponse"/> class.
        /// </summary>
        /// <param name="teamName">Team name.</param>
        /// <param name="vehicleModel">Vehicle model.</param>
        /// <param name="vehicleManufacturingDate">Vehicle manufacturing date.</param>
        public VehicleResponse(string teamName, string vehicleModel, DateTime vehicleManufacturingDate)
        {
            _teamName = teamName;
            _vehicleModel = vehicleModel;
            _vehicleManufacturingDate = vehicleManufacturingDate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Team name.
        /// </summary>
        public string TeamName { get { return _teamName; } set { _teamName = value; } }

        /// <summary>
        /// Vehicle model.
        /// </summary>
        public string VehicleModel { get { return _vehicleModel; } set { _vehicleModel = value; } }

        /// <summary>
        /// Vehicle manufacturing date.
        /// </summary>
        public DateTime VehicleManufacturingDate { get { return _vehicleManufacturingDate; } set { _vehicleManufacturingDate = value; } }

        #endregion
    }
}
