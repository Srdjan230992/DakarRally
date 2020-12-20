using DakarRally.Exceptions;
using DakarRally.Models;
using DakarRally.Repository;
using Microsoft.Extensions.Logging;
using System;
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

        private DakarRallyRepository dakarRally;
        private Random random = new Random();
        private readonly object raceLock = new object();
        private const int startFinishDistance = 10000; // 10000km
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
            dakarRally = new DakarRallyRepository(context);
            _logger = logger;
        }

        #endregion

        #region Public methods

        /// <inheritdoc/>
        public async Task<Race> CreateRace(int year)
        {
            var race = new Race(year);
            if(VakidateRaceExistence(race.Year)) { throw new InvalidStateException($"[{nameof(RaceServiceImpl)}] Race with year: {year} already exists!"); }
            dakarRally.Races.Insert(race);
            await SaveChanges();
            return FindRaceByYear(year);
        }

        /// <inheritdoc/>
        public async Task<Vehicle> AddVehicleToRace(Vehicle vehicle, int raceId)
        {
            ValidateVehicle(vehicle, raceId);
            vehicle.RaceId = raceId;
            dakarRally.Vehicles.Insert(vehicle);
            await SaveChanges();
            return FindVehicleById(vehicle.Id);
        }

        /// <inheritdoc/>
        public async Task UpdateVehicleInfo(Vehicle vehicle, long id)
        {
            if (!CheckIfVehicleExists(id)) { throw new VehiclesNotFoundException($"[{nameof(RaceServiceImpl)}] Vehicle with id: {id} does not exist!"); }
            if (!dakarRally.Races.Any(x => x.Year == FindRaceIdForVehicle(id) && x.State == RaceState.PENDING)) { throw new VehicleNotModifiedException($"[{nameof(RaceServiceImpl)}] Race with race id: {vehicle.RaceId} needs to be in pending state!"); }
            await SaveChanges();
        }

        /// <inheritdoc/>
        public async Task DeleteVehicle(long id)
        {
            var vehicle = dakarRally.Vehicles.GetByID(id);

            if (vehicle == null) { throw new VehiclesNotFoundException($"[{nameof(RaceServiceImpl)}] Vehicle with {id} is not found"); }

            CheckIfRaceIsInPendingState(FindRaceIdForVehicle(id));
            dakarRally.Vehicles.Delete(vehicle);
            await SaveChanges();
        }

        /// <inheritdoc/>
        public Race StartRace(int raceId)
        {
            lock (raceLock)
            {
                _logger.LogInformation($"[{nameof(RaceServiceImpl)}] Race with {raceId} started.");

                if (CheckIfAnyRaceIsRunning()) throw new InvalidStateException($"[{nameof(RaceServiceImpl)}] Any race is already running!");

                bool allVehiclesFinished = false;
                var startTime = DateTime.Now;
                int rank = 0;
                var vehicles = FindVehiclesWithRaceId(raceId);

                if (vehicles.Count() == 0) throw new RacesNotFoundException($"[{nameof(RaceServiceImpl)}] Race with {raceId} is not found");

                SetRaceState(raceId, RaceState.RUNNING);
                do
                {
                    Thread.Sleep(1000); // 1h
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

                SetRaceState(raceId, RaceState.FINISHED);

                _logger.LogInformation($"[{nameof(RaceServiceImpl)}] Race with {raceId} finished.");
            }
            return FindRaceByYear(raceId);
        }

        /// <inheritdoc/>
        public bool CheckIfAnyRaceIsRunning()
        {
            return dakarRally.Races.All(x => x.State != RaceState.RUNNING) ? false : true;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Increases vehicle distance.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        private void IncreaseVehicleDistance(Vehicle vehicle)
        {
            vehicle.PassedDistance += (vehicle.PassedDistance + vehicle.VehicleSpeed > startFinishDistance) ? (startFinishDistance - vehicle.PassedDistance) : vehicle.VehicleSpeed;
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
            if (vehicle.VehicleStatus == VehicleStatus.NOSTATUS)
            {
                vehicle.VehicleStatus = VehicleStatus.FINISHRACE;
            }
            UpdateDatabase(vehicle);
        }

        /// <summary>
        /// Checks if race exists and is in pennding state.
        /// </summary>
        /// <param name="year">Race id.</param>
        /// <returns>true if race is in pending state.</returns>
        private void CheckIfRaceIsInPendingState(long year)
        {
            if (!dakarRally.Races.Any(x => x.Year == year && x.State == RaceState.PENDING))
            {
                throw new InvalidStateException($"[{nameof(RaceServiceImpl)}] Race with race id: {year} needs to be in pending state!");
            }
        }

        /// <summary>
        /// Validates vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        /// <param name="raceId">Race id.</param>
        /// <returns>true if vehicle is valid.</returns>
        private void ValidateVehicle(Vehicle vehicle, int raceId)
        {
            if(!VakidateRaceExistence(raceId)) { throw new InvalidStateException($"[{nameof(RaceServiceImpl)}] Race with year: {raceId} does not exist!"); }
            CheckIfRaceIsInPendingState(raceId);
        }

        /// <summary>
        /// Checks if race exists.
        /// </summary>
        /// <param name="id">Race id.</param>
        /// <returns>true if race exists.</returns>
        private bool VakidateRaceExistence(long id)
        {
            return dakarRally.Races.Any(e => e.Year == id) ? true : false;
        }

        /// <summary>
        /// Checks if vehicle exists.
        /// </summary>
        /// <param name="id">vehicle id.</param>
        /// <returns>true if vehicle exists.</returns>
        private bool CheckIfVehicleExists(long id)
        {
            return dakarRally.Vehicles.Any(e => e.Id == id);
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
                vehicle.VehicleStatus = VehicleStatus.BREAKDOWN;
                vehicle.VehicleManufaturingDate = DateTime.Now.ToLocalTime();
                vehicle.Rank = 0;
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
                vehicle.VehicleManufaturingDate = DateTime.Now.ToLocalTime();
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
            dakarRally.Races.Get(id => id.Year == raceId).First().State = state;
            dakarRally.Commit();
        }

        /// <summary>
        /// Find vehicle with race id.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        /// <returns>Desired vehicle.</returns>
        private IQueryable<Vehicle> FindVehiclesWithRaceId(int raceId)
        {
            return dakarRally.Vehicles.Get(id => id.RaceId == raceId).AsQueryable();
        }

        /// <summary>
        /// Checks if all vehicles finish the race.
        /// </summary>
        /// <param name="raceId">Race id.</param>
        /// <returns>true id race is finished.</returns>
        private bool CheckIfRaceFinished(int raceId)
        {
            return dakarRally.Vehicles.Get(id => id.RaceId == raceId).All(x => x.VehicleFinishedRace == true) ? true : false;
        }

        /// <summary>
        /// Find race by id.
        /// </summary>
        /// <param name="year">Race year.</param>
        /// <returns>Race.</returns>
        private Race FindRaceByYear(int year)
        {
            var race = dakarRally.Races.Get(x => x.Year == year).FirstOrDefault();
            return race != null ? race : throw new RacesNotFoundException($"Race with year: {year} is not found!");
        }

        /// <summary>
        /// Find vehicle by id.
        /// </summary>
        /// <param name="id">Vehicle id.</param>
        /// <returns>Vehicle.</returns>
        private Vehicle FindVehicleById(long id)
        {
            var vehicle = dakarRally.Vehicles.Get(x => x.Id == id).FirstOrDefault();
            return vehicle != null ? vehicle : throw new VehiclesNotFoundException($"Vehicle with id: {id} is not found!");
        }

        /// <summary>
        /// Find race id for vehicle id.
        /// </summary>
        /// <param name="vehicleId">Vehicle id.</param>
        /// <returns>Race id.</returns>
        private long FindRaceIdForVehicle(long vehicleId)
        {
            var vehicle = dakarRally.Vehicles.Get(id => id.Id == vehicleId).FirstOrDefault();
            return vehicle != null ? vehicle.RaceId : throw new InvalidStateException($"Vehicle with id: {vehicleId} has invalid data. Race id can not be null!");
        }

        /// <summary>
        /// Updates database.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        private void UpdateDatabase(Vehicle vehicle)
        {
            dakarRally.Vehicles.Update(vehicle);
            dakarRally.Commit();
        }

        /// <summary>
        /// Save changes to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        private async Task SaveChanges()
        {
            try
            {
                await dakarRally.CommitAsync();
            }
            catch (Exception)
            {
                throw new InvalidStateException($"[{nameof(RaceServiceImpl)}] Error occured while saving to the database.");
            }
        }

        #endregion
    }
}