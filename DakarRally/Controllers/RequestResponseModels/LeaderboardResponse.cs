﻿using System;

namespace DakarRally.Controllers
{
    /// <summary>
    /// LeaderboardResponse class;
    /// </summary>
    public class LeaderboardResponse
    {
        #region Fields

        private string _teamName;
        private string _vehicleModel;
        private DateTime _vehicleManufacturingDate;
        private int _distance;
        private int _rank;
        private string _status;
        private bool _isLightMalfunctionOccured;
        private bool _isHeavyMalfunctionOccured;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardResponse"/> class.
        /// </summary>
        /// <param name="teamName">Team name.</param>
        /// <param name="vehicleModel">Vehicle model.</param>
        /// <param name="vehicleManufacturingDate">Vehicle manufacturing date.</param>
        /// <param name="distance">Passed distance.</param>
        /// <param name="rank">Vehicle rank in the race.</param>
        /// <param name="status">Vehicle status.</param>
        /// <param name="isLightMalfunctionOccured">Is light malfunction occured indication.</param>
        /// <param name="isHeavyMalfunctionOccured">Is heavy malfunction occured indication.</param>
        public LeaderboardResponse(string teamName, string vehicleModel, DateTime vehicleManufacturingDate, int distance, int rank, string status, bool isLightMalfunctionOccured, bool isHeavyMalfunctionOccured)
        {
            _teamName = teamName;
            _vehicleModel = vehicleModel;
            _vehicleManufacturingDate = vehicleManufacturingDate;
            _distance = distance;
            _rank = rank;
            _status = status;
            _isLightMalfunctionOccured = isLightMalfunctionOccured;
            _isHeavyMalfunctionOccured = isHeavyMalfunctionOccured;
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

        /// <summary>
        /// Distance.
        /// </summary>
        public int Distance { get { return _distance; } set { _distance = value; } }

        /// <summary>
        /// Rank.
        /// </summary>
        public int Rank { get { return _rank; } set { _rank = value; } }

        /// <summary>
        /// Vehicle status.
        /// </summary>
        public string Status { get { return _status; } set { _status = value; } }

        /// <summary>
        /// Is light malfunction ocuured indication.
        /// </summary>
        public bool IsLightMalfunctionOccured { get { return _isLightMalfunctionOccured; } set { _isLightMalfunctionOccured = value; } }

        /// <summary>
        /// Is heavy malfunction ocuured indication.
        /// </summary>
        public bool IsHeavyMalfunctionOccured { get { return _isHeavyMalfunctionOccured; } set { _isHeavyMalfunctionOccured = value; } }

        #endregion
    }
}