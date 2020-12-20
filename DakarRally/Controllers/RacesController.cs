using DakarRally.Models;
using DakarRally.Helper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DakarRally.Services;
using Microsoft.AspNetCore.Http;

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
        // POST: api/races/2021
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateActionParameters]
        public async Task<ActionResult<Race>> CreateRace([Range(1970, 2050)][FromBody] int year)
        {
            var race = await _raceService.CreateRace(year);
            return CreatedAtAction(nameof(CreateRaceResponse), race);
        }

        /// <summary>
        /// Add vehicle to the race.
        /// </summary>
        /// <param name="raceId">Race id (year).</param>
        /// <param name="vehicle">New vehicle.</param>
        /// <returns>Vehicle added to the race.</returns>
        // POST: api/races/2021/vehicles
        [HttpPost("{raceId}/vehicles")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateActionParameters]
        public async Task<ActionResult<VehiclesResponse>> AddVehicleToRace([Range(1970, 2050)] int raceId, [ModelBinder(BinderType = typeof(VehicleTypeModelBinder))][FromBody] Vehicle vehicleRequest)
        {
            var vehicle = await _raceService.AddVehicleToRace(vehicleRequest, raceId);
            return CreatedAtAction(nameof(CreateVehicleResponse), vehicle);
        }

        /// <summary>
        /// Updates vehicle informations.
        /// </summary>
        /// <param name="vehicle">Vehicle informations.</param>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Updated vehicle informations.</returns>
        // PUT: api/races/vehicles/1
        [HttpPut("vehicles/{id}")]
        [ProducesResponseType(StatusCodes.Status304NotModified)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateVehicle([ModelBinder(BinderType = typeof(VehicleTypeModelBinder))][FromBody] Vehicle vehicle, int id)
        {
            await _raceService.UpdateVehicleInfo(vehicle, id);
            return Ok(new VehiclesResponse(vehicle.TeamName, vehicle.VehicleModel, vehicle.VehicleManufaturingDate));
        }

        /// <summary>
        /// Delete vehicle.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Information if vehicle is deleted.</returns>
        // DELETE: api/races/vehicles/1
        [HttpDelete("vehicles/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteVehicle(int id)
        {
            await _raceService.DeleteVehicle(id);
            return NoContent();
        }

        /// <summary>
        /// Starts the race.
        /// </summary>
        /// <param name="raceId">Race id (year).</param>
        // PUT: api/races/2021/start
        [HttpPut("{year}/start")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ValidateActionParameters]
        public ActionResult StartRace([Range(1970, 2050)] int year)
        {
            var race = _raceService.StartRace(year);
            return Ok(race);
        }

        /// <summary>
        /// Find Race by race id.
        /// </summary>
        /// <param name="id">Race id.</param>
        /// <returns>The race.</returns>
        [HttpGet("response")]
        public ActionResult<Race> CreateRaceResponse(Race race)
        {
            return race;
        }

        /// <summary>
        /// Create respones.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>The vehicle.</returns>
        [HttpGet("vehicles/response")]
        public ActionResult<VehiclesResponse> CreateVehicleResponse(Vehicle vehicle)
        {
            return new VehiclesResponse(vehicle.TeamName, vehicle.VehicleModel, vehicle.VehicleManufaturingDate);
        }

        #endregion
    }
}