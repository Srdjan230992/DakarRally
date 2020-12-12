using DakarRally.Helper;
using DakarRally.Interfaces;
using DakarRally.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DakarRally.Helper.AppHelper;

namespace DakarRally.Services
{
    public class RaceInformationsManager : IRaceInformationsManager
    {
        private readonly VehicleDbContext _context;
        private List<string> vehicleTypes = new List<string>(5) { "sportcar", "terraincar", "sportmotorbike", "crossmotorbike", "truck" };
        private List<VehicleStatus> vehicleStatuses = new List<VehicleStatus>(3) { VehicleStatus.NoStatus, VehicleStatus.FinishRace, VehicleStatus.BreakDown };
        public RaceInformationsManager(VehicleDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetLeaderboardForAllVehicles()
        {
            var race = FindRaceInRaunningState();
            if (race != null)
            {
                return await FindVeaclesFromRace(race.Year).OrderBy(x => x.Rank).ToListAsync();
            }
            return null;
        }

        public async Task<List<Vehicle>> GetLeaderboardForVehicle(string type)
        {
            var race = FindRaceInRaunningState();
            if (race != null)
            {
                switch (type.ToLower())
                {
                    case "cars":
                        return await FindVeaclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type == "sportcar" || t.Type == "terraincar").ToListAsync();
                    case "motorcycles":
                        return await FindVeaclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type == "sportmotorbike" || t.Type == "crossmotorbike").ToListAsync();
                    case "trucks":
                        return await FindVeaclesFromRace(race.Year).OrderBy(x => x.Rank).Where(t => t.Type == "truck").ToListAsync();
                }
            }
            return null;
        }

        public VehicleStatistic GetVehicleStatistics(long vehicleId)
        {
            var vehicle = _context.Vehicles.Where(v => v.Id == vehicleId).FirstOrDefault();
            if (vehicle != null)
            {
                return new VehicleStatistic(vehicle.PassedDistance, vehicle.IsHeavyMalfunctionOccured, vehicle.LightMalfunctionCount);
            }
            return null;
        }

        public async Task<FilterOutputModel> FindVehicles(FilterData filterData, string order)
        {
            var vahiclesByTeamName = await _context.Vehicles.Where(x => x.TeamName == filterData.Team).OrderBy(x => x.TeamName).ToListAsync();
            if (vahiclesByTeamName == null) return null;
            var vahiclesByModel = FilterData(filterData.Model.LogicOperation, vahiclesByTeamName, _context.Vehicles.Where(x => x.VehicleModel == filterData.Model.Field));
            if (vahiclesByModel == null) return null;
            var vahiclesByStatus = FilterData(filterData.Status.LogicOperation, vahiclesByModel, _context.Vehicles.Where(x => x.VehicleStatus.ToString() == filterData.Status.Field));
            if (vahiclesByStatus == null) return null;
            var vahiclesByDistance = FilterData(filterData.Distance.LogicOperation, vahiclesByStatus, _context.Vehicles.Where(x => x.PassedDistance.ToString() == filterData.Distance.Field));
            if (vahiclesByDistance == null) return null;

            var filterOutputModel = new FilterOutputModel();
            filterOutputModel.Count = vahiclesByDistance?.Count();
            filterOutputModel.Items = vahiclesByDistance;
            return filterOutputModel;
        }

        private IEnumerable<Vehicle> SortData(string order, IEnumerable<Vehicle> l1, string Field)
        {
            if(order == "asc")
            {
                //return l1.OrderBy(x=>x.fiels)
            }
            else if(order == "desc"){

            }
            return null;
        }

        private IEnumerable<Vehicle> FilterData(string operation, IEnumerable<Vehicle> l1, IEnumerable<Vehicle> l2)
        {
            operation = operation.ToLowerInvariant();
            if (!string.IsNullOrEmpty(operation) && IsValidOperation(operation))
            {
                if (operation == "and")
                {
                    return l1.Intersect(l2);
                }
                else if (operation == "or")
                {
                    return l1.Union(l2);
                }
            }
            return null;
        }

        private bool IsValidOperation(string operation)
        {
            return (operation == "and" || operation == "or") ? true : false;
        }
        private IEnumerable<T> Get<T>(Func<T, object> selector, string value) where T : class
        {
            return _context.Set<T>().Where(x => selector(x).ToString() == value).Distinct().AsEnumerable();
        }
        public RaceStatus GetRaceStatus(int raceId)
        {
            RaceStatus status = new RaceStatus();
            var races = _context.Races.Where(v => v.Year == raceId);
            if (races != null)
            {
                status.Status = races.FirstOrDefault().State.ToString();
            }
            foreach(var vehicleStatus in vehicleStatuses)
            {
                var vehicles = _context.Vehicles.Where(v => v.VehicleStatus == vehicleStatus);
                if (vehicles != null)
                {
                    status.VehicleStatusToNumberOfVehicles.Add(vehicleStatus.ToString(), vehicles.Count());
                }
            }
            foreach(var type in vehicleTypes)
            {
                var vehicles = _context.Vehicles.Where(v => v.Type == type);
                if (vehicles != null)
                {
                    status.VehicleTypeToNumberOfVehicles.Add(type, vehicles.Count());
                }
            }
            
            return status;
        }

        private IQueryable<Vehicle> FindVeaclesFromRace(int year)
        {
            return _context.Vehicles.Where(v => v.RaceId == year);
        }

        private Race FindRaceInRaunningState()
        {
            return _context.Races.Where(r => r.State == Helper.AppHelper.RaceState.Running || r.State == Helper.AppHelper.RaceState.Finished).FirstOrDefault();
        }
    }
}
