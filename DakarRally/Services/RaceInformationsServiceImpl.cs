using DakarRally.Helper;
using DakarRally.Interfaces;
using DakarRally.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DakarRally.Helper.AppEnums;

namespace DakarRally.Services
{
    /// <summary>
    /// RaceInformationsServiceImpl class.
    /// </summary>
    public class RaceInformationsServiceImpl : IRaceInformationsService
    {
        #region Fields

        private readonly VehicleDbContext _context;
        private readonly ILogger _logger;
        private readonly List<string> vehicleTypes = new List<string>(5) { "sportcar", "terraincar", "sportmotorbike", "crossmotorbike", "truck" };
        private readonly List<VehicleStatus> vehicleStatuses = new List<VehicleStatus>(3) { VehicleStatus.NoStatus, VehicleStatus.FinishRace, VehicleStatus.BreakDown };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceInformationsServiceImpl"/> class.
        /// </summary>
        /// <param name="context">Vehicle database context.</param>
        public RaceInformationsServiceImpl(ILogger<RaceInformationsServiceImpl> logger, VehicleDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        #endregion

        #region Public methods

        /// <inheritdoc/>
        public async Task<List<Vehicle>> GetLeaderboardForAllVehicles()
        {
            var race = FindRaceInRaunningState();
            return await FindVehiclesFromRace(race.Year)?.OrderBy(x => x.Rank).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Vehicle>> GetLeaderboardForVehicle(string type)
        {
            var race = FindRaceInRaunningState();
            switch (type.ToLower())
            {
                case "cars":
                    return await FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "sportcar" || t.Type.ToLower() == "terraincar")?.ToListAsync();
                case "motorcycles":
                    return await FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "sportmotorbike" || t.Type.ToLower() == "crossmotorbike")?.ToListAsync();
                case "trucks":
                    return await FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "truck")?.ToListAsync();
                default:
                    _logger.LogInformation("Invalid race type!");
                    throw new Exception($"Invalid race type: {type}!");
            }
        }

        /// <inheritdoc/>
        public VehicleStatistic GetVehicleStatistics(long vehicleId)
        {
            var vehicle = _context.Vehicles.Where(v => v.Id == vehicleId)?.FirstOrDefault();
            return vehicle != null ? new VehicleStatistic(vehicle.PassedDistance, vehicle.IsHeavyMalfunctionOccured, vehicle.LightMalfunctionCount) : throw new NullReferenceException($"Vehicle with id: {vehicleId} does not exist!");
        }

        /// <inheritdoc/>
        public async Task<DesiredVehiclesResponse> FindVehicles(DesiredVehiclesRequest filterData, string order)
        {
            var vehiclesByTeamName = await SortData(order, _context.Vehicles.Where(x => x.TeamName == filterData.Team))?.ToListAsync();
            var vehiclesByModel = await SortData(order, FilterData(filterData.Model.LogicOperation, vehiclesByTeamName, _context.Vehicles.Where(x => x.VehicleModel == filterData.Model.Field)).AsQueryable())?.ToListAsync();
            var vehiclesByStatus = await SortData(order, FilterData(filterData.Status.LogicOperation, vehiclesByModel, _context.Vehicles.Where(x => x.VehicleStatus.ToString() == filterData.Status.Field)).AsQueryable())?.ToListAsync();
            var vehiclesByDistance = await SortData(order, FilterData(filterData.Distance.LogicOperation, vehiclesByStatus, _context.Vehicles.Where(x => x.PassedDistance.ToString() == filterData.Distance.Field)).AsQueryable())?.ToListAsync();

            var filterOutputModel = new DesiredVehiclesResponse();
            filterOutputModel.Count = vehiclesByDistance?.Count();
            filterOutputModel.Items = vehiclesByDistance;
            return filterOutputModel;
        }

        /// <inheritdoc/>
        public RaceStatus GetRaceStatus(int raceId)
        {
            var status = new RaceStatus();
            var race = _context.Races.Where(v => v.Year == raceId);
            if (race != null)
            {
                status.Status = race.FirstOrDefault()?.State.ToString();
            }
            else
            {
                _logger.LogError($"Race with race id: {raceId} is not found!");
                throw new NullReferenceException($"Race with race id: {raceId} is not found!");
            }

            PopulateVehicleStatusToNumberOfVehicles(status);
            PopulateVehicleTypeToNumberOfVehicles(status);
           
            return status;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Vehicle status to number of vehicles mapping.
        /// </summary>
        /// <param name="status">Race status.</param>
        private void PopulateVehicleStatusToNumberOfVehicles(RaceStatus status)
        {
            foreach (var vehicleStatus in vehicleStatuses)
            {
                var vehicles = _context.Vehicles.Where(v => v.VehicleStatus == vehicleStatus);
                if (vehicles != null)
                {
                    status.VehicleStatusToNumberOfVehicles.Add(vehicleStatus.ToString(), vehicles.Count());
                }
            }
        }

        /// <summary>
        /// Vehicle type to number of vehicles mapping.
        /// </summary>
        /// <param name="status">Race status.</param>
        private void PopulateVehicleTypeToNumberOfVehicles(RaceStatus status)
        {
            foreach (var type in vehicleTypes)
            {
                var vehicles = _context.Vehicles.Where(v => v.Type == type);
                if (vehicles != null)
                {
                    status.VehicleTypeToNumberOfVehicles.Add(type, vehicles.Count());
                }
            }
        }

        /// <summary>
        /// Sorts list of vehicles.
        /// </summary>
        /// <param name="order">Sort order (asc/desc).</param>
        /// <param name="vehicles">List of vehicles for sorting.</param>
        /// <returns>Sorted list of vehicles.</returns>
        private IQueryable<Vehicle> SortData(string order, IQueryable<Vehicle> vehicles)
        {
            if(vehicles == null)
            {
                throw new ArgumentNullException("There is no vehicles for desired filtering criteria!");
            }

            order = order.ToLowerInvariant();

            if (string.IsNullOrEmpty(order) && (order != "asc" || order != "desc"))
            {
                throw new Exception("Invalid order operation!");
            }
            return order == "desc" ? vehicles.OrderByDescending(x => x.TeamName) : vehicles.OrderBy(x => x.TeamName);
        }

        /// <summary>
        /// Filter data.
        /// </summary>
        /// <param name="operation">Logic operation (and/or).</param>
        /// <param name="vehicles1">First list for filtering.</param>
        /// <param name="vehicles2">Second list for filtering.</param>
        /// <returns>Filter results.</returns>
        private IEnumerable<Vehicle> FilterData(string operation, IEnumerable<Vehicle> vehicles1, IQueryable<Vehicle> vehicles2)
        {
            operation = operation.ToLowerInvariant();

            return IsValidOperation(operation) ? (operation == "and" ? vehicles1.Intersect(vehicles2) : vehicles1.Union(vehicles2)) : null;
        }

        /// <summary>
        /// Determines if logic operation is valid.
        /// </summary>
        /// <param name="operation">Logic operation (and/or).</param>
        /// <returns>true if operation is valid.</returns>
        private bool IsValidOperation(string operation)
        {
            return (operation == "and" || operation == "or") ? true : false;
        }

        /// <summary>
        /// Find vehicles from race.
        /// </summary>
        /// <param name="year">Race year.</param>
        /// <returns>List of vehicles.</returns>
        private IQueryable<Vehicle> FindVehiclesFromRace(int year)
        {
            var vehicles = _context.Vehicles.Where(v => v.RaceId == year);
            return vehicles != null ? vehicles : throw new ArgumentNullException($"Vehicles with race id: {year} are not found!");
        }

        /// <summary>
        /// Find race in running or finished state.
        /// </summary>
        /// <returns>The race.</returns>
        private Race FindRaceInRaunningState()
        {
            var races = _context.Races.Where(r => r.State == RaceState.Running);
            return races != null ? races.FirstOrDefault() : throw new Exception("Race in running state is not found!");
        }

        #endregion
    }
}
