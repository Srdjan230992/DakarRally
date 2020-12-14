using DakarRally.Models;
using DakarRally.Helper;
using DakarRally.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        // POST: api/Races/Race/1970
        [HttpPost("race/{year}")]
        [ValidateActionParameters]
        public async Task<ActionResult<Race>> Race([Range(1970, 2050)] int year)
        {
            var race = new Race(year);
            if (await _raceService.CreateRace(race) != 0)
            {    
                return CreatedAtAction(nameof(Race), new { id = race.Year }, race);
            }
            return BadRequest("It is not possible to create race!");
        }

        /// <summary>
        /// Add vehicle to the race.
        /// </summary>
        /// <param name="vehicle">New vehicle.</param>
        /// <returns>Vehicle added to the race.</returns>
        // POST: api/Races/Vehicle
        [HttpPost("vehicle")]
        public async Task<ActionResult<Vehicle>> Vehicle([ModelBinder(BinderType = typeof(VehicleTypeModelBinder))][FromBody] Vehicle vehicle)
        {
            if(await _raceService.AddVehicleToRace(vehicle) != 0)
            {
                return CreatedAtAction(nameof(Viecle), new { id = vehicle.Id }, vehicle);
            }
            return BadRequest("It is not possible to add vehicle to the race!");
        }

        /// <summary>
        /// Updates vehicle informations.
        /// </summary>
        /// <param name="vehicle">Vehicle informations.</param>
        /// <returns>Updated vehicle informations.</returns>
        // PUT: api/Races/Vehicle/
        [HttpPut("vehicle")]
        public async Task<IActionResult> VehicleInfo([ModelBinder(BinderType = typeof(VehicleTypeModelBinder))][FromBody] Vehicle vehicle)
        {
            if (await _raceService.UpdateVehicleInfo(vehicle) != 0)
            {
                return Ok(vehicle);
            }
            return BadRequest("It is not possible to update vehicle!");
        }

        /// <summary>
        /// Delete vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Information if vehicle is deleted.</returns>
        // DELETE: api/Races/Vehicles/5
        [HttpDelete("vehicle/{id}")]
        public async Task<ActionResult> Vehicle(long id)
        {
            if (await _raceService.DeleteVehicle(id) != 0)
            {
                return Ok("Vehicle is successfully deleted!");
            }
            return BadRequest("It is not possible to delete Vehicle. Vehicle with the entered id does not exist or Race is not in pannding state.");
        }

        /// <summary>
        /// Starts the race.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        // GET: api/Races/StartRace/2020
        [HttpGet("race/{id}/start")]
        [ValidateActionParameters]
        public ActionResult StartRace([Range(1970, 2050)] int id)
        {
            if (_raceService.CheckIfAnyRaceIsRunning()) { return BadRequest("Any race is already running!"); }

            _raceService.StartRace(id);

            return Ok("Race successfully finished.");
        }

        /// <summary>
        /// Find Race by race id.
        /// </summary>
        /// <param name="id">Race id.</param>
        /// <returns>The race.</returns>
        // GET: api/Races/Race/id
        [HttpGet("race/{id}")]
        [ValidateActionParameters]
        public async Task<ActionResult<Race>> Race([Range(1970, 2050)] long id)
        {
            var race = await _raceService.FindRaceById(id);
            return race == null ? NotFound("Race with desired id is not found!") : Ok(race);
        }

        /// <summary>
        /// Find vehicle by id.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>The vehicle.</returns>
        // GET: api/Races/Viecle/id
        [HttpGet("vehicle/{id}")]
        public async Task<ActionResult<Vehicle>> Viecle(long id)
        {
            var vehicle = await _raceService.FindVehicleById(id);
            return vehicle == null ? NotFound("Vehicle with desired id is not found!") : Ok(vehicle);
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