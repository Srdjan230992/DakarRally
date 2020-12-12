using DakarRally.Interfaces;
using DakarRally.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static DakarRally.Helper.AppHelper;

namespace DakarRally.Services
{
    public class RaceManager : IRaceManager
    {
        private readonly VehicleDbContext _context;
        private Random random = new Random();
        private readonly object raceLock = new object();
        private const int startFinishDistance = 10000; //10000km

        public RaceManager(VehicleDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateRace(Race race)
        {
            if (ValidateRace(race))
            {
                _context.Races.Add(race);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> AddVehicleToRace(Vehicle vehicle)
        {
            if (ValidateVehicle(vehicle))
            {
                _context.Vehicles.Add(vehicle);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> UpdateVehicleInfo(Vehicle vehicle)
        {
            if (vehicle != null && CheckIfVehicleExists(vehicle.Id) && CheckIfRaceIsInPenndingState(vehicle.RaceId))
            {
                try
                {
                    ExcludePropertiesFromUpdate(vehicle);
                    return await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return 0;
        }

        public async Task<int> DeleteVehicle(long id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle != null && CheckIfRaceIsInPenndingState(vehicle.RaceId))
            {
                _context.Vehicles.Remove(vehicle);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
        public void StartRace(int raceId)
        {
            lock (raceLock)
            {
                bool allVehiclesFinished = false;
                var startTime = DateTime.Now;

                SetRaceState(raceId, RaceState.Running);
                int rank = 0;
                var vehicles = FindVehiclesWithRaceId(raceId);
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
                                    if (vehicle.PassedDistance + vehicle.VehicleSpeed > startFinishDistance)
                                    {
                                        vehicle.PassedDistance += startFinishDistance - vehicle.PassedDistance;
                                    }
                                    else
                                    {
                                        vehicle.PassedDistance += vehicle.VehicleSpeed;
                                    }

                                    if ((vehicle.PassedDistance >= startFinishDistance || CalculateHeavyMalfunctionProbability(vehicle)))
                                    {
                                        vehicle.Rank = rank++;
                                        vehicle.VehicleFinishedRace = true;
                                        if(vehicle.VehicleStatus == VehicleStatus.NoStatus)
                                        {
                                            vehicle.VehicleStatus = VehicleStatus.FinishRace;
                                        }
                                        UpdateDatabase(vehicle);
                                        allVehiclesFinished = CheckIfRaceFinished(raceId);
                                        if (allVehiclesFinished) break;
                                    }
                                    UpdateDatabase(vehicle);
                                }
                            }
                            if (vehicle.LightMalfunctionCounter != 0)
                            {
                                vehicle.LightMalfunctionCounter--;
                            }
                        }
                    }
                } while (!allVehiclesFinished);

                SetRaceState(raceId, RaceState.Finished);
            }
        }

         public async Task<Race> FindRaceById(long raceId)
        {
            return await _context.Races.FindAsync(raceId);
        }
       
        public async Task<Vehicle> FindVehicleById(long vehicleId)
        {
            return await _context.Vehicles.FindAsync(vehicleId);
        }
        /// <summary>
        /// Exists and is in pennding state.
        /// </summary>
        /// <param name="raceId"></param>
        /// <returns></returns>
        private bool CheckIfRaceIsInPenndingState(long raceId)
        {
            return _context.Races.Any(x => x.Year == raceId && x.State == RaceState.Pending);
        }
        private bool ValidateRace(Race race)
        {
            return (race != null && CheckIfYearIsValid(race.Year) && !CheckIfRaceExists(race.Year)) ? true : false;
        }
        private bool ValidateVehicle(Vehicle vehicle)
        {
            return vehicle != null && CheckIfRaceExists(vehicle.RaceId) && !CheckIfVehicleExists(vehicle.Id) && CheckIfRaceIsInPenndingState(vehicle.RaceId);
        }
        private bool CheckIfRaceExists(long id)
        {
            return _context.Races.Any(e => e.Year == id);
        }
        private bool CheckIfVehicleExists(long id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
        private bool CheckIfYearIsValid(int year)
        {
            return (year >= 1970 && year <= 2050) ? true : false;
        }
        public bool CalculateHeavyMalfunctionProbability(Vehicle vehicle)
        {
            if (random.NextDouble() < vehicle.HeavyMalfunctionProbability)
            {
                vehicle.IsHeavyMalfunctionOccured = true;
                vehicle.VehicleStatus = VehicleStatus.BreakDown;
                return true;
            }
            return false;
        }
        public bool CalculateLightMalfunctionProbability(Vehicle vehicle)
        {
            if (random.NextDouble() < vehicle.LightMalfunctionProbability)
            {
                vehicle.IsLightMalfunctionOccured = true;
                return true;
            }
            return false;
        }
        public bool CheckIfAnyRaceIsRunning()
        {
            return _context.Races.All(x => x.State != RaceState.Running) ? false : true;
        }
        private void SetRaceState(int raceId, RaceState state)
        {
            _context.Races.Where(id => id.Year == raceId).First().State = state;
            _context.SaveChangesAsync();
        }
        public IQueryable<Vehicle> FindVehiclesWithRaceId(int raceId)
        {
            return _context.Vehicles.Where(id => id.RaceId == raceId);
        }
        public bool CheckIfRaceFinished(int raceId)
        {
            return _context.Vehicles.Where(id => id.RaceId == raceId).All(x => x.VehicleFinishedRace == true) ? true : false;
        }
        public void UpdateDatabase(Vehicle vehicle)
        {
            _context.Entry(vehicle).State = EntityState.Modified;//update baze
            _context.SaveChangesAsync();
        }
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
