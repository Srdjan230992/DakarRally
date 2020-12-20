using DakarRally.Exceptions;
using DakarRally.Helper;
using DakarRally.Models;
using DakarRally.Repository;
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

        private DakarRallyRepository dakarRally;
        private const string DESC = "desc";
        private const string AND = "and";
        private const string SPORTCAR = "sportcar";
        private const string TERRAINCAR = "terraincar";
        private const string SPORTMOTORBIKE = "sportmotorbike";
        private const string CROSSMOTORBIKE = "crossmotorbike";
        private const string TRUCK = "truck";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RaceInformationsServiceImpl"/> class.
        /// </summary>
        /// <param name="context">Vehicle database context.</param>
        public RaceInformationsServiceImpl(VehicleDbContext context)
        {
            dakarRally = new DakarRallyRepository(context);
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
            var vehicleType = (VehiclesType)Enum.Parse(typeof(VehiclesType), type.ToUpperInvariant());
            switch (vehicleType)
            {
                case VehiclesType.CARS:
                    return FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == SPORTCAR || t.Type.ToLower() == TERRAINCAR)?.ToList();
                case VehiclesType.MOTORCYCLES:
                    return FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == SPORTMOTORBIKE || t.Type.ToLower() == CROSSMOTORBIKE)?.ToList();
                case VehiclesType.TRUCKS:
                    return FindVehiclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type.ToLower() == TRUCK)?.ToList();
                default:
                    throw new Exception($"[{nameof(RaceInformationsServiceImpl)}] Invalid race type: {type}!");
            }
        }

        /// <inheritdoc/>
        public VehicleStatistic GetVehicleStatistics(long vehicleId)
        {
            var vehicle = dakarRally.Vehicles.Get(v => v.Id == vehicleId).FirstOrDefault();
            return vehicle != null ? new VehicleStatistic(vehicle.PassedDistance, vehicle.IsHeavyMalfunctionOccured, vehicle.LightMalfunctionCount) : throw new VehiclesNotFoundException($"Vehicle with id: {vehicleId} does not exist!");
        }

        /// <inheritdoc/>
        public FilteredVehiclesResponse FindVehicles(VehiclesRequest filterData, string order)
        {
            ValidateFilterParameters(filterData, order);

            var vehiclesByTeamName = SortData(order, dakarRally.Vehicles.Get(x => x.TeamName == filterData.Team).AsQueryable())?.ToList();
            var vehiclesByModel = SortData(order, FilterData(filterData.Model.LogicOperation, vehiclesByTeamName, dakarRally.Vehicles.Get(x => x.VehicleModel == filterData.Model.Field).AsQueryable()).AsQueryable())?.ToList();
            var vehiclesByStatus = SortData(order, FilterData(filterData.Status.LogicOperation, vehiclesByModel, dakarRally.Vehicles.Get(x => x.VehicleStatus.ToString() == filterData.Status.Field).AsQueryable()).AsQueryable())?.ToList();
            var vehiclesByDistance = SortData(order, FilterData(filterData.Distance.LogicOperation, vehiclesByStatus, dakarRally.Vehicles.Get(x => x.PassedDistance.ToString() == filterData.Distance.Field).AsQueryable()).AsQueryable())?.ToList();

            var filterOutputModel = new FilteredVehiclesResponse();
            filterOutputModel.Count = vehiclesByDistance?.Count();
            filterOutputModel.Items = vehiclesByDistance;
            return filterOutputModel;
        }

        /// <inheritdoc/>
        public RaceStatus GetRaceStatus(int raceId)
        {
            var status = new RaceStatus();
            var race = dakarRally.Races.Get(v => v.Year == raceId).FirstOrDefault();

            if (race == null)
            {
                throw new RacesNotFoundException($"[{nameof(RaceInformationsServiceImpl)}] Race with race id: {raceId} is not found!");              
            }

            status.Status = race.State.ToString();
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
            foreach (var vehicleStatus in (VehicleStatus[])Enum.GetValues(typeof(VehicleStatus)))
            {
                var vehicles = dakarRally.Vehicles.Get(v => v.VehicleStatus == vehicleStatus);
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
            foreach (var type in Enum.GetNames(typeof(VehicleType)))
            {
                var vehicles = dakarRally.Vehicles.Get(v => v.Type == type);
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

            return order == DESC ? vehicles.OrderByDescending(x => x.TeamName) : vehicles.OrderBy(x => x.TeamName);
        }

        /// <summary>
        /// Filter data.
        /// </summary>
        /// <param name="operation">Logic operation (and/or).</param>
        /// <param name="firstFilterData">First list for filtering.</param>
        /// <param name="secondFilterData">Second list for filtering.</param>
        /// <returns>Filter results.</returns>
        private IEnumerable<Vehicle> FilterData(string operation, IEnumerable<Vehicle> firstFilterData, IQueryable<Vehicle> secondFilterData)
        {
            return operation.ToLowerInvariant() == AND ? firstFilterData.Intersect(secondFilterData) : firstFilterData.Union(secondFilterData);
        }

        /// <summary>
        /// Find vehicles from race.
        /// </summary>
        /// <param name="year">Race year.</param>
        /// <returns>List of vehicles.</returns>
        private IQueryable<Vehicle> FindVehiclesFromRace(int year)
        {
            var vehicles = dakarRally.Vehicles.Get(v => v.RaceId == year).AsQueryable();
            return vehicles.Count() != 0 ? vehicles : throw new VehiclesNotFoundException($"[{nameof(RaceInformationsServiceImpl)}] Vehicles with race id: {year} are not found!");
        }

        /// <summary>
        /// Find race in running or finished state.
        /// </summary>
        /// <returns>The race.</returns>
        private Race FindRaceInRaunningState()
        {
            var races = dakarRally.Races.Get(r => r.State == RaceState.RUNNING).FirstOrDefault();
            
            return races != null ? races : throw new RacesNotFoundException($"[{nameof(RaceInformationsServiceImpl)}] Race in running state is not found!");
        }

        /// <summary>
        /// Validates filter data.
        /// </summary>
        /// <param name="filterData">Filter data.</param>
        /// <param name="order">Sort order.</param>
        private void ValidateFilterParameters(VehiclesRequest filterData, string order)
        {
            if (order == null || filterData.Distance == null || filterData.ManufacturingDate == null || filterData.Model == null || filterData.Status == null || filterData.Team == null)
            {
                throw new ArgumentNullException($"[{nameof(RaceInformationsServiceImpl)}] All filter criterias need to be entered!");
            }
        }

        #endregion
    }
}