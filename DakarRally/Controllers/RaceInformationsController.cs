using DakarRally.Helper;
using DakarRally.Interfaces;
using DakarRally.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DakarRally.Controllers
{
    [Route("api/RaceInformations")]
    [ApiController]
    public class RaceInformationsController : ControllerBase
    {
        private readonly IRaceInformationsManager _raceInformationsManager;

        public RaceInformationsController(IRaceInformationsManager raceInformationsManager)
        {
            _raceInformationsManager = raceInformationsManager;
        }

        // GET: api/RaceInformations/LeaderboardForAllVehicles
        [HttpGet("LeaderboardForAllVehicles")]
        public async Task<ActionResult<List<Vehicle>>> GetLeaderboardForAllVehicles()
        {
            var vehicles = await _raceInformationsManager.GetLeaderboardForAllVehicles();
            return CreatedAtAction(nameof(Leaderboard), new { vehicles = vehicles }, vehicles); ;
        }

        // GET: api/RaceInformations/LeaderboardForVehicle/cars
        [HttpGet("LeaderboardForVehicle/{type}")]
        public async Task<ActionResult<List<Vehicle>>> GetLeaderboardForVehicle(string type)
        {
            var vehicles = await _raceInformationsManager.GetLeaderboardForVehicle(type);
            return CreatedAtAction(nameof(Leaderboard), new { vehicles = vehicles }, vehicles);
        }
        // GET: api/RaceInformations/VehicleStatistics/2
        [HttpGet("VehicleStatistics/{vehicleId}")]
        public VehicleStatistic GetVehicleStatistics(long vehicleId)
        {
            return _raceInformationsManager.GetVehicleStatistics(vehicleId);
        }
        // GET: api/RaceInformations/RaceStatus/2020
        [HttpGet("RaceStatus/{raceId}")]
        public RaceStatus GetRaceStatus(int raceId)
        {
            return _raceInformationsManager.GetRaceStatus(raceId);
        }
        // GET: api/RaceInformations/FindVehicle
        [HttpGet("FindVehicle/{order}")]
        public async Task<ActionResult<FilterOutputModel>> FindVehicle([FromBody] FilterData filterData, string order)
        {
            var filterOutput = await _raceInformationsManager.FindVehicles(filterData, order);
            if(filterOutput == null)
            {
                return NotFound();
            }
            return filterOutput;
        }

        // GET: api/RaceInformations/Leaderboard
        [HttpGet("Leaderboard")]
        public async Task<ActionResult<List<Vehicle>>> Leaderboard(IQueryable<Vehicle> vehicles)
        {
            var v = await vehicles.ToListAsync();
            if (v == null)
            {
                return NotFound();
            }
            return v;
        }

    }
}
