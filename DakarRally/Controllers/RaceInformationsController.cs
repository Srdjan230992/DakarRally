using DakarRally.Helper;
using DakarRally.Interfaces;
using DakarRally.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            return vehicles != null ? Ok(vehicles) : NotFound("Leaderboard is not found!");
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
            return vehicles != null ? Ok(vehicles) : NotFound("Leaderboard is not found!");
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
            return vehicleStatistic != null ? Ok(vehicleStatistic) : NotFound("Vehicle statistic is not found!");
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
            return raceStatus != null ? Ok(raceStatus) : NotFound("Race is not found!");
        }

        /// <summary>
        /// Retrives filtered data.
        /// </summary>
        /// <param name="filterData">Filter data.</param>
        /// <param name="order">Sort order (asc or desc).</param>
        /// <returns>Filter output model.</returns>
        // GET: api/informations/vehicles/desc
        [HttpPost("vehicles/{order}")]
        [ValidateOrderParameter]
        public async Task<ActionResult<FilterOutputModel>> FilterVehicles([FromBody] FilterData filterData, string order)
        {
            var filterOutput = await _raceInformationsService.FindVehicles(filterData, order);
            return filterOutput != null ? Ok(filterOutput) : NotFound("Vehicles are not found!");
        }

        #endregion
    }
}
