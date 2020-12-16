using DakarRally.Helper;
using DakarRally.Models;
using DakarRally.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Threading.Tasks;

namespace DakarRally.Controllers
{
    /// <summary>
    /// The race informations controller.
    /// </summary>
    [Route("api/informations")]
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
        // GET: api/informations/vehicles/leaderboard
        [HttpGet("vehicles/leaderboard")]
        public async Task<ActionResult<List<Vehicle>>> LeaderboardForAllVehicles()
        {
            var vehicles = await _raceInformationsService.GetLeaderboardForAllVehicles();
            return Ok(vehicles);
        }

        /// <summary>
        /// Retrives leaderboard informations for specific vehicle type.
        /// </summary>
        /// <param name="type">Vehicle type.</param>
        /// <returns>Vehicle with leaderboard informations.</returns>
        // GET: api/informations/vehicles/cars/leaderboard
        [HttpGet("vehicles/{type}/leaderboard")]
        [ValidateTypeParameter]
        public async Task<ActionResult<List<Vehicle>>> LeaderboardForVehicle(string type)
        {
            var vehicles = await _raceInformationsService.GetLeaderboardForVehicle(type);
            return vehicles != null ? Ok(vehicles) : NotFound("Vehicles are not found!");
        }

        /// <summary>
        /// Retrives vehicle statistics for desired vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Vehicle statistics.</returns>
        // GET: api/informations/vehicles/2/statistics
        [HttpGet("vehicles/{id}/statistics")]
        public ActionResult<VehicleStatistic> VehicleStatistics(long id)
        {
            var vehicleStatistic = _raceInformationsService.GetVehicleStatistics(id);
            return Ok(vehicleStatistic);
        }

        /// <summary>
        /// Retrives race status.
        /// </summary>
        /// <param name="id">Race id (year).</param>
        /// <returns>Race status.</returns>
        // GET: api/informations/races/2020/status
        [HttpGet("races/{id}/status")]
        [ValidateActionParameters]
        public ActionResult<RaceStatus> RaceStatus([Range(1970, 2050)] int id)
        {
            var raceStatus =_raceInformationsService.GetRaceStatus(id);
            return Ok(raceStatus);
        }

        /// <summary>
        /// Retrives filtered data.
        /// </summary>
        /// <param name="request">Filter informatiions.</param>
        /// <param name="order">Sort order (asc or desc).</param>
        /// <returns>Filter output model.</returns>
        // GET: api/informations/vehicles/desc
        [HttpPost("vehicles/{order}")]
        [ValidateOrderParameter]
        public async Task<ActionResult<DesiredVehiclesResponse>> FilterVehicles([FromBody] DesiredVehiclesRequest request, string order)
        {
            var desiredVehicles = await _raceInformationsService.FindVehicles(request, order);
            return Ok(desiredVehicles);
        }

        #endregion
    }
}
