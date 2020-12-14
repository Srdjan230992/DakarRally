using DakarRally.Helper;
using DakarRally.Interfaces;
using DakarRally.Models;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<Vehicle>> GetLeaderboardForAllVehicles()
        {
            var race = FindRaceInRaunningState();
            return race != null ? await FindVehiclesFromRace(race.Year)?.OrderBy(x => x.Rank).ToListAsync() : null;
        }

        /// <inheritdoc/>
        public async Task<List<Vehicle>> GetLeaderboardForVehicle(string type)
        {
            var race = FindRaceInRaunningState();
            switch (type.ToLower())
            {
                case "cars":
                    return await FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "sportcar" || t.Type.ToLower() == "terraincar").ToListAsync();
                case "motorcycles":
                    return await FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "sportmotorbike" || t.Type.ToLower() == "crossmotorbike").ToListAsync();
                case "trucks":
                    return await FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == "truck").ToListAsync();
                default:
                    throw new Exception("Race type is not valid!");
            }
            throw new NullReferenceException("Race is not found!");
        }

        /// <inheritdoc/>
        public VehicleStatistic GetVehicleStatistics(long vehicleId)
        {
            var vehicle = _context.Vehicles.Where(v => v.Id == vehicleId)?.FirstOrDefault();
            return vehicle != null ? new VehicleStatistic(vehicle.PassedDistance, vehicle.IsHeavyMalfunctionOccured, vehicle.LightMalfunctionCount) : null;
        }

        /// <inheritdoc/>
        public async Task<FilterOutputModel> FindVehicles(FilterData filterData, string order)//createvehicleresponse
        {
            var vehiclesByTeamName = await SortData(order, _context.Vehicles.Where(x => x.TeamName == filterData.Team)).ToListAsync();
            if (vehiclesByTeamName == null) return null;
            var vehiclesByModel = await SortData(order, FilterData(filterData.Model.LogicOperation, vehiclesByTeamName, _context.Vehicles.Where(x => x.VehicleModel == filterData.Model.Field)).AsQueryable()).ToListAsync();
            if (vehiclesByModel == null) return null;
            var vehiclesByStatus = await SortData(order, FilterData(filterData.Status.LogicOperation, vehiclesByModel, _context.Vehicles.Where(x => x.VehicleStatus.ToString() == filterData.Status.Field)).AsQueryable()).ToListAsync();
            if (vehiclesByStatus == null) return null;
            var vehiclesByDistance = await SortData(order, FilterData(filterData.Distance.LogicOperation, vehiclesByStatus, _context.Vehicles.Where(x => x.PassedDistance.ToString() == filterData.Distance.Field)).AsQueryable()).ToListAsync();
            if (vehiclesByDistance == null) return null;

            var filterOutputModel = new FilterOutputModel();
            filterOutputModel.Count = vehiclesByDistance?.Count();
            filterOutputModel.Items = vehiclesByDistance;
            return filterOutputModel;
        }

        /// <inheritdoc/>
        public RaceStatus GetRaceStatus(int raceId)
        {
            RaceStatus status = new RaceStatus();
            var race = _context.Races.Where(v => v.Year == raceId);
            if (race != null)
            {
                status.Status = race.FirstOrDefault()?.State.ToString();
            }
            else
            {
                throw new NullReferenceException("Race not found!");
            }
            foreach (var vehicleStatus in vehicleStatuses)
            {
                var vehicles = _context.Vehicles.Where(v => v.VehicleStatus == vehicleStatus);
                if (vehicles != null)
                {
                    status.VehicleStatusToNumberOfVehicles.Add(vehicleStatus.ToString(), vehicles.Count());
                }
            }
            foreach (var type in vehicleTypes)
            {
                var vehicles = _context.Vehicles.Where(v => v.Type == type);
                if (vehicles != null)
                {
                    status.VehicleTypeToNumberOfVehicles.Add(type, vehicles.Count());
                }
            }
            return status;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sorts list of vehicles.
        /// </summary>
        /// <param name="order">Sort order (asc/desc).</param>
        /// <param name="vehicles">List of vehicles for sorting.</param>
        /// <returns>Sorted list of vehicles.</returns>
        private IQueryable<Vehicle> SortData(string order, IQueryable<Vehicle> vehicles)
        {
            order = order.ToLowerInvariant();

            if (string.IsNullOrEmpty(order) && (order != "asc" || order != "desc"))
            {
                return null;
            }

            if (order == "desc")
            {
                return vehicles.OrderByDescending(x => x.TeamName);
            }
            return vehicles.OrderBy(x => x.TeamName);
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
            if (!string.IsNullOrEmpty(operation) && IsValidOperation(operation))
            {
                if (operation == "and")
                {
                    return vehicles1.Intersect(vehicles2);
                }
                else if (operation == "or")
                {
                    return vehicles1.Union(vehicles2);
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if logic operation is valid.
        /// </summary>
        /// <param name="operation">Logic operation (and/or).</param>
        /// <returns>true if operation is valid.</returns>
        private bool IsValidOperation(string operation)
        {
            return (operation.ToLower() == "and" || operation.ToLower() == "or") ? true : false;
        }

        /// <summary>
        /// Find vehicles from race.
        /// </summary>
        /// <param name="year">Race year.</param>
        /// <returns>List of vehicles.</returns>
        private IQueryable<Vehicle> FindVehiclesFromRace(int year)
        {
            var vehicles = _context.Vehicles.Where(v => v.RaceId == year);
            return vehicles != null ? vehicles : throw new ArgumentNullException("Vehicles with desired race id are not found!");
        }

        /// <summary>
        /// Find race in running or finished state.
        /// </summary>
        /// <returns>The race.</returns>
        private Race FindRaceInRaunningState()
        {
            var races = _context.Races.Where(r => r.State == RaceState.Running);
            return races != null ? races.FirstOrDefault() : throw new ArgumentNullException("Race in running state is not found!");
        }

        #endregion
    }
}
