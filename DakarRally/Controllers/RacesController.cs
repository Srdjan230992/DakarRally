using DakarRally.Models;
using DakarRally.Helper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DakarRally.Services;

namespace DakarRally.Controllers
{
    /// <summary>
    /// The race controller.
    /// </summary>
    [Route("api/races")]
    [ApiController]
    public class RacesController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// Race service instance.
        /// </summary>
        private readonly IRaceService _raceService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RacesController"/> class.
        /// </summary>
        /// <param name="raceService">Provides race service instance.</param>
        public RacesController(IRaceService raceService)
        {
            _raceService = raceService;
        }

        #endregion

        #region Routes

        /// <summary>
        /// Creates a new race.
        /// </summary>
        /// <param name="year">Race year.</param>
        /// <returns>Race informations.</returns>
        // POST: api/races/race/1970
        [HttpPost("race/{year}")]
        [ValidateActionParameters]
        public async Task<ActionResult<Race>> Race([Range(1970, 2050)] int year)
        {
            var race = new Race(year);
            return await _raceService.CreateRace(race) != 0 ? CreatedAtAction(nameof(Race), new { id = race.Year }, race) : BadRequest($"Race with {year} is not created!");
        }

        /// <summary>
        /// Add vehicle to the race.
        /// </summary>
        /// <param name="vehicle">New vehicle.</param>
        /// <returns>Vehicle added to the race.</returns>
        // POST: api/races/vehicle
        [HttpPost("vehicle")]
        public async Task<ActionResult<VehicleResponse>> Vehicle([ModelBinder(BinderType = typeof(VehicleTypeModelBinder))][FromBody] Vehicle vehicle)
        {
            return await _raceService.AddVehicleToRace(vehicle) != 0 ? CreatedAtAction(nameof(CreateResponse), new { id = vehicle.Id }, vehicle) : BadRequest($"Vehicle with {vehicle.Id} is not added to the race!");
        }

        /// <summary>
        /// Updates vehicle informations.
        /// </summary>
        /// <param name="vehicle">Vehicle informations.</param>
        /// <returns>Updated vehicle informations.</returns>
        // PUT: api/races/vehicle/
        [HttpPut("vehicle")]
        public async Task<IActionResult> VehicleInfo([ModelBinder(BinderType = typeof(VehicleTypeModelBinder))][FromBody] Vehicle vehicle)
        {
            return  await _raceService.UpdateVehicleInfo(vehicle) != 0 ? Ok(new VehicleResponse(vehicle.TeamName, vehicle.VehicleModel, vehicle.VehicleManufaturingDate)) : BadRequest("Vehicle is not updated!");
        }

        /// <summary>
        /// Delete vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Information if vehicle is deleted.</returns>
        // DELETE: api/races/vehicle/5
        [HttpDelete("vehicle/{id}")]
        public async Task<ActionResult> Vehicle(long id)
        {
            return await _raceService.DeleteVehicle(id) != 0 ? Ok("Vehicle is successfully deleted!") : BadRequest($"Vehicle with {id} is not deleted!");
        }

        /// <summary>
        /// Starts the race.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        // GET: api/race/2020/start
        [HttpGet("race/{id}/start")]
        [ValidateActionParameters]
        public ActionResult StartRace([Range(1970, 2050)] int id)
        {
            if (_raceService.CheckIfAnyRaceIsRunning()) { return BadRequest("Any race is already running!"); }
            _raceService.StartRace(id);
            return Ok($"Race with id: {id} is successfully finished.");
        }

        /// <summary>
        /// Find Race by race id.
        /// </summary>
        /// <param name="id">Race id.</param>
        /// <returns>The race.</returns>
        // GET: api/race/id
        [HttpGet("race/{id}")]
        [ValidateActionParameters]
        public async Task<ActionResult<Race>> Race([Range(1970, 2050)] long id)
        {
            var race = await _raceService.FindRaceById(id);
            return race == null ? NotFound($"Race with id: {id} is not found!") : Ok(race);
        }

        /// <summary>
        /// Find vehicle by id.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>The vehicle.</returns>
        // GET: api/vehicle/2
        [HttpGet("vehicle/{id}")]
        public async Task<ActionResult<VehicleResponse>> CreateResponse(long id)
        {
            var vehicle = await _raceService.FindVehicleById(id);
            return vehicle == null ? NotFound($"Vehicle with id: {id} is not found!") : Ok(new VehicleResponse(vehicle.TeamName, vehicle.VehicleModel, vehicle.VehicleManufaturingDate));
        }

        // GET: api/Races/PopulateInitData
        [HttpGet("PopulateInitData")]
        public void PopulateInitData()
        {
            _raceService.PopulateInitData();
        }

        #endregion
    }
}