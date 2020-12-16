using DakarRally.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static DakarRally.Helper.AppEnums;

namespace DakarRally.Services
{
    /// <summary>
    /// RaceServiceImpl class.
    /// </summary>
    public class RaceServiceImpl : IRaceService
    {
        #region Fields

        private readonly VehicleDbContext _context;
        private Random random = new Random();
        private readonly object raceLock = new object();
        private const int startFinishDistance = 10000; //10000km
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceServiceImpl"/> class.
        /// </summary>
        /// <param name="context">Race database context.</param>
        /// <param name="logger">Logger instance.</param>
        public RaceServiceImpl(VehicleDbContext context, ILogger<RaceServiceImpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        #region Public methods

        /// <inheritdoc/>
        public async Task<int> CreateRace(Race race)
        {
            CheckIfRaceExists(race.Year);
            _context.Races.Add(race);
            return await SaveChanges();
        }

        /// <inheritdoc/>
        public async Task<int> AddVehicleToRace(Vehicle vehicle)
        {
            if (ValidateVehicle(vehicle))
            {
                _context.Vehicles.Add(vehicle);
                return await SaveChanges();
            }
            return 0;
        }

        /// <inheritdoc/>
        public async Task<int> UpdateVehicleInfo(Vehicle vehicle)
        {
            if (!CheckIfVehicleExists(vehicle.Id)) throw new NullReferenceException($"[{nameof(RaceServiceImpl)}] Vehicle with id: {vehicle.Id} does not exist!");
            CheckIfRaceIsInPendingState(vehicle.RaceId);
            ExcludePropertiesFromUpdate(vehicle);
            return await SaveChanges();
        }

        /// <inheritdoc/>
        public async Task<int> DeleteVehicle(long id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null) throw new NullReferenceException($"[{nameof(RaceServiceImpl)}] Vehicle with {id} is not found");

            CheckIfRaceIsInPendingState(vehicle.RaceId);
            _context.Vehicles.Remove(vehicle);
            return await SaveChanges();
        }

        /// <inheritdoc/>
        public void StartRace(int raceId)
        {
            lock (raceLock)
            {
                _logger.LogInformation($"[{nameof(RaceServiceImpl)}] Race with {raceId} rtarted.");

                bool allVehiclesFinished = false;
                var startTime = DateTime.Now;
                int rank = 0;
                var vehicles = FindVehiclesWithRaceId(raceId);

                SetRaceState(raceId, RaceState.Running);
                do
                {
                    Thread.Sleep(1000); //1h
                    foreach (var vehicle in vehicles)
                    {
                        if (!vehicle.VehicleFinishedRace)
                        {
                            if (vehicle.LightMalfunctionCounter == 0)
                            {
                                if (CalculateLightMalfunctionProbability(vehicle))
                                {
                                    vehicle.LightMalfunctionCounter = vehicle.LightMalfunctionDelay;
                                    vehicle.LightMalfunctionCount++;
                                }
                                if (vehicle.LightMalfunctionCounter == 0)
                                {
                                    IncreaseVehicleDistance(vehicle);
                                    if ((vehicle.PassedDistance >= startFinishDistance || CalculateHeavyMalfunctionProbability(vehicle)))
                                    {
                                        vehicle.Rank = rank++;
                                        FinishRace(vehicle);
                                        allVehiclesFinished = CheckIfRaceFinished(raceId);
                                        if (allVehiclesFinished) break;
                                    }
                                    UpdateDatabase(vehicle);
                                }
                            }
                            DecreaseLightMalfunctionCounter(vehicle);
                        }
                    }
                } while (!allVehiclesFinished);

                SetRaceState(raceId, RaceState.Finished);

                _logger.LogInformation($"[{nameof(RaceServiceImpl)}] Race with {raceId} finished.");
            }
        }

        /// <inheritdoc/>
        public async Task<Race> FindRaceById(long raceId)
        {
            return await _context.Races.FindAsync(raceId);
        }

        /// <inheritdoc/>
        public async Task<Vehicle> FindVehicleById(long vehicleId)
        {
            return await _context.Vehicles.FindAsync(vehicleId);
        }

        /// <inheritdoc/>
        public bool CheckIfAnyRaceIsRunning()
        {
            return _context.Races.All(x => x.State != RaceState.Running) ? false : true;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Increases vehicle distance.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        private void IncreaseVehicleDistance(Vehicle vehicle)
        {
            if (vehicle.PassedDistance + vehicle.VehicleSpeed > startFinishDistance)
            {
                vehicle.PassedDistance += startFinishDistance - vehicle.PassedDistance;
            }
            else
            {
                vehicle.PassedDistance += vehicle.VehicleSpeed;
            }
        }

        /// <summary>
        /// Decreases malfunction counter for vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        private void DecreaseLightMalfunctionCounter(Vehicle vehicle)
        {
            if (vehicle.LightMalfunctionCounter != 0)
            {
                vehicle.LightMalfunctionCounter--;
            }
        }
        /// <summary>
        /// Finish the race.
        /// </summary>
        /// <param name="vehicle">Vehicle/</param>
        private void FinishRace(Vehicle vehicle)
        {
            vehicle.VehicleFinishedRace = true;
            if (vehicle.VehicleStatus == VehicleStatus.NoStatus)
            {
                vehicle.VehicleStatus = VehicleStatus.FinishRace;
            }
            UpdateDatabase(vehicle);
        }

        /// <summary>
        /// Checks if race exists and is in pennding state.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        /// <returns>true if race is in pending state.</returns>
        private bool CheckIfRaceIsInPendingState(long raceId)
        {
            return _context.Races.Any(x => x.Year == raceId && x.State == RaceState.Pending) ? true : throw new Exception($"[{nameof(RaceServiceImpl)}] Race with race id: {raceId} needs to be in pending state!");
        }

        /// <summary>
        /// Validates vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <returns>true if vehicle is valid.</returns>
        private bool ValidateVehicle(Vehicle vehicle)
        {
            if (CheckIfVehicleExists(vehicle.Id)) throw new Exception($"[{nameof(RaceServiceImpl)}] Vehicle with id: {vehicle.Id} already exist!");
            return (CheckIfRaceExists(vehicle.RaceId) && CheckIfRaceIsInPendingState(vehicle.RaceId)) ? true : false;
        }

        /// <summary>
        /// Checks if race exists.
        /// </summary>
        /// <param name="id">Race id.</param>
        /// <returns>true if race exists.</returns>
        private bool CheckIfRaceExists(long id)
        {
            var race = _context.Races.Any(e => e.Year == id);
            return race ? true : throw new Exception($"[{nameof(RaceServiceImpl)}] Race with year: {id} does not exist!");
        }

        /// <summary>
        /// Checks if vehicle exists.
        /// </summary>
        /// <param name="id">vehicle id.</param>
        /// <returns>true if vehicle exists.</returns>
        private bool CheckIfVehicleExists(long id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }

        /// <summary>
        /// Calculates heavy malfunction probability.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <returns>true if heavy malfunction occured.</returns>
        private bool CalculateHeavyMalfunctionProbability(Vehicle vehicle)
        {
            if (random.NextDouble() < vehicle.HeavyMalfunctionProbability)
            {
                vehicle.IsHeavyMalfunctionOccured = true;
                vehicle.VehicleStatus = VehicleStatus.BreakDown;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates light malfunction probability.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <returns>true if light malfunction occured.</returns>
        private bool CalculateLightMalfunctionProbability(Vehicle vehicle)
        {
            if (random.NextDouble() < vehicle.LightMalfunctionProbability)
            {
                vehicle.IsLightMalfunctionOccured = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets race state.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        /// <param name="state">Desired race state.</param>
        private void SetRaceState(int raceId, RaceState state)
        {
            _context.Races.Where(id => id.Year == raceId).First().State = state;
            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Find vehicle with race id.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        /// <returns>Desired vehicle.</returns>
        private IQueryable<Vehicle> FindVehiclesWithRaceId(int raceId)
        {
            return _context.Vehicles.Where(id => id.RaceId == raceId);
        }

        /// <summary>
        /// Checks if all vehicles finish the race.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        /// <returns>true id race is finished.</returns>
        private bool CheckIfRaceFinished(int raceId)
        {
            return _context.Vehicles.Where(id => id.RaceId == raceId).All(x => x.VehicleFinishedRace == true) ? true : false;
        }

        /// <summary>
        /// Updates database.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        private void UpdateDatabase(Vehicle vehicle)
        {
            _context.Entry(vehicle).State = EntityState.Modified;
            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Save changes to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        private async Task<int> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception($"[{nameof(RaceServiceImpl)}] Error occured while saving to the database.");
            }
        }

        /// <summary>
        /// Excludes from update properties which will be dynamicaly changed.
        /// </summary>
        /// <param name="vehicle">Vehicle for update.</param>
        private void ExcludePropertiesFromUpdate(Vehicle vehicle)
        {
            _context.Entry(vehicle).State = EntityState.Modified;
            _context.Entry(vehicle).Property(x => x.HeavyMalfunctionProbability).IsModified = false;
            _context.Entry(vehicle).Property(x => x.IsHeavyMalfunctionOccured).IsModified = false;
            _context.Entry(vehicle).Property(x => x.IsLightMalfunctionOccured).IsModified = false;
            _context.Entry(vehicle).Property(x => x.LightMalfunctionDelay).IsModified = false;
            _context.Entry(vehicle).Property(x => x.LightMalfunctionProbability).IsModified = false;
            _context.Entry(vehicle).Property(x => x.PassedDistance).IsModified = false;
            _context.Entry(vehicle).Property(x => x.Type).IsModified = false;
            _context.Entry(vehicle).Property(x => x.VehicleFinishedRace).IsModified = false;
            _context.Entry(vehicle).Property(x => x.VehicleManufaturingDate).IsModified = false;
            _context.Entry(vehicle).Property(x => x.VehicleSpeed).IsModified = false;
        }

        #endregion

        public void PopulateInitData()
        {
            Race race = new Race(2020);
            race.Id = 1;
            _context.Races.Add(race);

            List<Vehicle> vehicles = new List<Vehicle>(25);
            int j = 1;
            for (int i = 0; i < 10; i++)
            {
                var sc = new SportCar();
                sc.Id = j++;
                sc.RaceId = 2020;
                sc.TeamName = "Team" + i;
                sc.Type = "sportcar";
                sc.VehicleModel = "model1";
                vehicles.Add(sc);
            }
            for (int i = 0; i < 5; i++)
            {
                var sc = new CrossMotorbike();
                sc.Id = j++;
                sc.RaceId = 2020;
                sc.TeamName = "Team" + i;
                sc.Type = "crossmotorbike";
                sc.VehicleModel = "model1";
                vehicles.Add(sc);
            }
            for (int i = 0; i < 5; i++)
            {
                var sc = new SportMotorbike();
                sc.Id = j++;
                sc.RaceId = 2020;
                sc.TeamName = "Team" + i;
                sc.Type = "sportmotorbike";
                sc.VehicleModel = "model1";
                sc.VehicleStatus = VehicleStatus.NoStatus;
                vehicles.Add(sc);
            }
            for (int i = 0; i < 5; i++)
            {
                var sc = new TerrainCar();
                sc.Id = j++;
                sc.RaceId = 2020;
                sc.TeamName = "Team" + i;
                sc.Type = "terraincar";
                sc.VehicleModel = "model1";
                vehicles.Add(sc);
            }
            for (int i = 0; i < 5; i++)
            {
                var sc = new Truck();
                sc.Id = j++;
                sc.RaceId = 2020;
                sc.TeamName = "Team" + i;
                sc.Type = "truck";
                sc.VehicleModel = "model1";
                vehicles.Add(sc);
            }
            _context.Vehicles.AddRange(vehicles);
            _context.SaveChangesAsync();
        }
    }
}
