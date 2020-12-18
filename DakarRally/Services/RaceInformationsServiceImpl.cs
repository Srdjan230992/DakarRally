using DakarRally.Exceptions;
using DakarRally.Helper;
using DakarRally.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<string> vehicleTypes = new List<string>(5) { "sportcar", "terraincar", "sportmotorbike", "crossmotorbike", "truck" };
        private readonly List<VehicleStatus> vehicleStatuses = new List<VehicleStatus>(3) { VehicleStatus.NoStatus, VehicleStatus.FinishRace, VehicleStatus.BreakDown };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceInformationsServiceImpl"/> class.
        /// </summary>
        /// <param name="context">Vehicle database context.</param>
        public RaceInformationsServiceImpl(VehicleDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Public methods

        /// <inheritdoc/>
        public List<Vehicle> GetLeaderboardForAllVehicles()
        {
            var race = FindRaceInRaunningState();
            return FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).ToList();
        }

        /// <inheritdoc/>
        public List<Vehicle> GetLeaderboardForVehicle(string type)
        {
            var race = FindRaceInRaunningState();
            switch (type.ToLower())
            {
                case "cars":
                    return FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "sportcar" || t.Type.ToLower() == "terraincar")?.ToList();
                case "motorcycles":
                    return FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "sportmotorbike" || t.Type.ToLower() == "crossmotorbike")?.ToList();
                case "trucks":
                    return FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "truck")?.ToList();
                default:
                    throw new Exception($"[{nameof(RaceInformationsServiceImpl)}] Invalid race type: {type}!");
            }
        }

        /// <inheritdoc/>
        public VehicleStatistic GetVehicleStatistics(long vehicleId)
        {
            var vehicle = _context.Vehicles.Where(v => v.Id == vehicleId)?.FirstOrDefault();
            return vehicle != null ? new VehicleStatistic(vehicle.PassedDistance, vehicle.IsHeavyMalfunctionOccured, vehicle.LightMalfunctionCount) : throw new VehiclesNotFoundException($"Vehicle with id: {vehicleId} does not exist!");
        }

        /// <inheritdoc/>
        public DesiredVehiclesResponse FindVehicles(DesiredVehiclesRequest filterData, string order)
        {
            var vehiclesByTeamName = SortData(order, _context.Vehicles.Where(x => x.TeamName == filterData.Team))?.ToList();
            var vehiclesByModel = SortData(order, FilterData(filterData.Model.LogicOperation, vehiclesByTeamName, _context.Vehicles.Where(x => x.VehicleModel == filterData.Model.Field)).AsQueryable())?.ToList();
            var vehiclesByStatus = SortData(order, FilterData(filterData.Status.LogicOperation, vehiclesByModel, _context.Vehicles.Where(x => x.VehicleStatus.ToString() == filterData.Status.Field)).AsQueryable())?.ToList();
            var vehiclesByDistance = SortData(order, FilterData(filterData.Distance.LogicOperation, vehiclesByStatus, _context.Vehicles.Where(x => x.PassedDistance.ToString() == filterData.Distance.Field)).AsQueryable())?.ToList();

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
                throw new RacesNotFoundException($"[{nameof(RaceInformationsServiceImpl)}] Race with race id: {raceId} is not found!");
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
                throw new VehiclesNotFoundException($"[{nameof(RaceInformationsServiceImpl)}] There is no vehicles for desired filtering criteria!");
            }

            order = order.ToLowerInvariant();

            if (string.IsNullOrEmpty(order) && (order != "asc" || order != "desc"))
            {
                throw new Exception($"[{nameof(RaceInformationsServiceImpl)}] Invalid order operation!");
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
            return vehicles.Count() != 0 ? vehicles : throw new VehiclesNotFoundException($"[{nameof(RaceInformationsServiceImpl)}] Vehicles with race id: {year} are not found!");
        }

        /// <summary>
        /// Find race in running or finished state.
        /// </summary>
        /// <returns>The race.</returns>
        private Race FindRaceInRaunningState()
        {
            var races = _context.Races.Where(r => r.State == RaceState.Running).FirstOrDefault();
            return races != null ? races : throw new Exception($"[{nameof(RaceInformationsServiceImpl)}] Race in running state is not found!");
        }

        #endregion
    }
}
