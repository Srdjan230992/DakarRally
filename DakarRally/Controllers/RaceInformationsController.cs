using DakarRally.Helper;
using DakarRally.Models;
using DakarRally.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DakarRally.Controllers
{
    /// <summary>
    /// The race informations controller.
    /// </summary>
    [Route("api/")]
    [ApiController]
    public class RaceInformationsController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// Race informations service instance.
        /// </summary>
        private readonly IRaceInformationsService _raceInformationsService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceInformationsController"/> class.
        /// </summary>
        /// <param name="raceInformationsService">Provides race informations service instance.</param>
        public RaceInformationsController(IRaceInformationsService raceInformationsService)
        {
            _raceInformationsService = raceInformationsService;
        }

        #endregion

        #region Routes

        /// <summary>
        /// Retrives leaderboard informations for all vehicles.
        /// </summary>
        /// <returns>List of vehicles with leaderboard informations.</returns>
        // GET: api/leaderboard
        [HttpGet("leaderboard")]
        public ActionResult<List<Vehicle>> LeaderboardForAllVehicles()
        {
            return Ok(_raceInformationsService.GetLeaderboardForAllVehicles());
        }

        /// <summary>
        /// Retrives leaderboard informations for specific vehicle type.
        /// </summary>
        /// <param name="type">Vehicle type.</param>
        /// <returns>Vehicle with leaderboard informations.</returns>
        // GET: api/?type=cars
        [HttpGet]
        [ValidateTypeParameter]
        public ActionResult<List<Vehicle>> LeaderboardForVehicle([FromQuery(Name = "type")] string type)
        {
            return Ok(_raceInformationsService.GetLeaderboardForVehicle(type));
        }

        /// <summary>
        /// Retrives vehicle statistics for desired vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Vehicle statistics.</returns>
        // GET: api/vehicles/2/statistics
        [HttpGet("vehicles/{id}/statistics")]
        public ActionResult<VehicleStatistic> VehicleStatistics(long id)
        {
            return Ok(_raceInformationsService.GetVehicleStatistics(id));
        }

        /// <summary>
        /// Retrives race status.
        /// </summary>
        /// <param name="id">Race id (year).</param>
        /// <returns>Race status.</returns>
        // GET: api/races/2020/status
        [HttpGet("races/{id}/status")]
        [ValidateActionParameters]
        public ActionResult<RaceStatus> RaceStatus([Range(1970, 2050)] int id)
        {
            return Ok(_raceInformationsService.GetRaceStatus(id));
        }

        /// <summary>
        /// Retrives filtered data.
        /// </summary>
        /// <param name="request">Filter informatiions.</param>
        /// <param name="order">Sort order (asc or desc).</param>
        /// <returns>Desired vehicles response.</returns>
        // GET: api/vehicles?order=desc
        [HttpPost("vehicles")]
        [ValidateOrderParameter]
        public ActionResult<DesiredVehiclesResponse> FilterVehicles([FromBody] DesiredVehiclesRequest request, [FromQuery(Name = "order")] string order)
        {
            return Ok(_raceInformationsService.FindVehicles(request, order));
        }

        #endregion
    }
}
