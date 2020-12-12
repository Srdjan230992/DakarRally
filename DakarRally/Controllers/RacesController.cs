using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DakarRally.Models;
using System.Threading;
using DakarRally.Helper;
using DakarRally.Interfaces;
using static DakarRally.Helper.AppHelper;

namespace DakarRally.Controllers
{
    [Route("api/Races")]
    [ApiController]
    public class RacesController : ControllerBase
    {
        private readonly IRaceManager _raceManager;

        public RacesController(IRaceManager raceManager)
        {
            _raceManager = raceManager;
        }

        // POST: api/Races/Race/1970
        [HttpPost("Race/{year}")]
        public async Task<ActionResult<Race>> CreateRace(int year)
        {
            var race = new Race(year);
            if (await _raceManager.CreateRace(race) != 0)
            {    
                return CreatedAtAction(nameof(FindRace), new { id = race.Year }, race);
            }
            return Content("It is not possible to create Race. Race with the entered year already exists or has invalid year.");
        }

        // POST: api/Races/Vehicle
        [HttpPost("Vehicle")]
        public async Task<ActionResult<Vehicle>> AddVehicleToRace([ModelBinder(BinderType = typeof(VehicleTypeModelBinder))][FromBody] Vehicle vehicle)
        {
            if(await _raceManager.AddVehicleToRace(vehicle) != 0)
            {
                return CreatedAtAction(nameof(FindViecle), new { id = vehicle.Id }, vehicle);
            }
            return Content("It is not possible to add Vehicle. Race with the entered race id does not exist or Race is not in pannding state.");
        }

        // PUT: api/Races/Vehicle/
        [HttpPut("Vehicle")]
        public async Task<IActionResult> UpdateVehicleInfo([ModelBinder(BinderType = typeof(VehicleTypeModelBinder))][FromBody] Vehicle vehicle)
        {
            if (await _raceManager.UpdateVehicleInfo(vehicle) != 0)
            {
                return CreatedAtAction(nameof(FindViecle), new { id = vehicle.Id }, vehicle);
            }
            return Content("It is not possible to update Vehicle. Vehicle with the entered id does not exist or Race is not in pannding state.");
        }

        // DELETE: api/Vehicles/5
        [HttpDelete("Vehicle/{id}")]
        public async Task<IActionResult> DeleteVehicle(long id)
        {
            if (await _raceManager.DeleteVehicle(id) != 0)
            {
                return Content("Viecle is successfully deleted!");
            }
            return Content("It is not possible to delete Vehicle. Vehicle with the entered id does not exist or Race is not in pannding state.");
        }

        // GET: api/Vehicles/StartRace/2020
        [HttpGet("StartRace/{raceId}")]
        public void StartRace(int raceId)
        {
            if (_raceManager.CheckIfAnyRaceIsRunning()) { return; }

            _raceManager.StartRace(raceId);
        }

        // GET: api/Races/FindRace/id
        [HttpGet("FindRace/{id}")]
        public async Task<ActionResult<Race>> FindRace(long id)
        {
            var race = await _raceManager.FindRaceById(id);
            if (race == null)
            {
                return NotFound();
            }
            return race;
        }

        // GET: api/Races/FindViecle/id
        [HttpGet("FindVehicle/{id}")]
        public async Task<ActionResult<Vehicle>> FindViecle(long id)
        {
            var race = await _raceManager.FindVehicleById(id);
            if (race == null)
            {
                return NotFound();
            }
            return race;
        }

        // GET: api/Races/PopulateInitData
        [HttpGet("PopulateInitData")]
        public void PopulateInitData()
        {
            _raceManager.PopulateInitData();
        }
    }
}