using DakarRally.Controller;
using DakarRally.Helper;
using DakarRally.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DakarRally.Controllers
{
    /// <summary>
    /// The race informations controller.
    /// </summary>
    [Route("api/races")]
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
        // GET: api/races/leaderboard
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("leaderboard")]
        public ActionResult<List<LeaderboardResponse>> FindLeaderboard()
        {
            return Ok(_raceInformationsService.GetLeaderboardForAllVehicles());
        }

        /// <summary>
        /// Retrives leaderboard informations for specific vehicle type.
        /// </summary>
        /// <param name="type">Vehicle type.</param>
        /// <returns>Vehicles with leaderboard informations.</returns>
        // GET: api/races/vehicles/leaderboard?type=cars
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("vehicles/leaderboard")]
        [ValidateTypeParameter]
        public ActionResult<List<LeaderboardResponse>> FindLeaderboardForVehicle([FromQuery(Name = "type")] string type)
        {
            return Ok(_raceInformationsService.GetLeaderboardForVehicle(type));
        }

        /// <summary>
        /// Retrives vehicle statistics for desired vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Vehicle statistics.</returns>
        // GET: api/races/vehicles/2/statistics
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("vehicles/{id}/statistics")]
        public ActionResult<VehicleStatistic> FindVehicleStatistics(long id)
        {
            return Ok(_raceInformationsService.GetVehicleStatistics(id));
        }

        /// <summary>
        /// Retrives race status.
        /// </summary>
        /// <param name="id">Race id (year).</param>
        /// <returns>Race status.</returns>
        // GET: api/races/2020/status
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}/status")]
        [ValidateActionParameters]
        public ActionResult<RaceStatus> GetRaceStatus([Range(1970, 2050)] int id)
        {
            return Ok(_raceInformationsService.GetRaceStatus(id));
        }

        /// <summary>
        /// Retrives filtered data.
        /// </summary>
        /// <param name="request">Filter informatiions.</param>
        /// <param name="order">Sort order (asc or desc).</param>
        /// <returns>Desired vehicles response.</returns>
        // POST: api/races/vehicles?order=desc
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("vehicles")]
        [ValidateOrderParameter]
        public ActionResult<FilteredVehiclesResponse> FilterVehicles([FromBody] VehiclesRequest request, [FromQuery(Name = "order")] string order)
        {
            return Ok(_raceInformationsService.FindVehicles(request, order));
        }

        #endregion
    }
}