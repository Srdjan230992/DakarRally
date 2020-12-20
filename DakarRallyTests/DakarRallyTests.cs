using DakarRally.Models;
using DakarRally.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static DakarRally.Helper.AppEnums;

namespace DakarRallyTests
{
    public class DakarRallyTests
    {
        private RaceInformationsServiceImpl informationsService;
        private RaceServiceImpl raceService;
        private DbContextOptions<VehicleDbContext> options;
        private static AutoResetEvent syncRace = new AutoResetEvent(false);

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<VehicleDbContext>().UseInMemoryDatabase(databaseName: "Vehicles").Options;           
        }

        [Test]
        public async Task AddUpdateDeleteVehicle_Test()
        {
            using (var context = new VehicleDbContext(options))
            {
                raceService = new RaceServiceImpl(context, new Mock<ILogger<RaceServiceImpl>>().Object);

                var sc = new SportCar();
                int raceId = 2020;
                CreateRace(context, raceId);

                await AddVehicleToTheRace(context, sc, raceId);

                await UpdateVehicleInfo(context, sc, raceId);

                await DeleteVehicle(context, sc, raceId);
            }
        }

        [Test]
        public void RaceInformationsInPandingState_Test()
        {
            using (var context = new VehicleDbContext(options))
            {
                int raceId = 2022;
                PopulateDatabase(raceId);
                informationsService = new RaceInformationsServiceImpl(context);

                // Check race status before the race is started
                var vs = informationsService.GetVehicleStatistics(2);
                Assert.AreEqual(0, vs.Distance);
                Assert.AreEqual(false, vs.IsHeayMalfanctionOssured);
                Assert.AreEqual(0, vs.LightMulfunctionCount);

                // Try to get leaderboard informations while there are no race in running state
                try
                {
                    var vehicles = informationsService.GetLeaderboardForAllVehicles();

                }catch(Exception ex)
                {
                    Assert.AreEqual("[RaceInformationsServiceImpl] Race in running state is not found!", ex.Message);
                }

                try
                {
                    var vehicles = informationsService.GetLeaderboardForVehicle("cars");

                }
                catch (Exception ex)
                {
                    Assert.AreEqual("[RaceInformationsServiceImpl] Race in running state is not found!", ex.Message);
                }

                // Check race status before race is started
                var rs = informationsService.GetRaceStatus(raceId);
                Assert.AreEqual(RaceState.PENDING.ToString(), rs.Status);
                foreach(var status in rs.VehicleStatusToNumberOfVehicles.Keys)
                {
                    Assert.AreEqual(rs.VehicleStatusToNumberOfVehicles[status], context.Vehicles.Where(x => x.VehicleStatus.ToString() == status).Count());
                }
                foreach (var type in rs.VehicleTypeToNumberOfVehicles.Keys)
                {
                    Vehicle veh = context.Vehicles.Where(x => x.Type == type).FirstOrDefault();
                    Assert.AreEqual(rs.VehicleTypeToNumberOfVehicles[type], context.Vehicles.Where(x => x.Type == type).Count());
                }
            }
        }

        [Test]
        public void RaceInformationsWhenRaceIsInFinishedState_Test()
        {
            using (var context = new VehicleDbContext(options))
            {
                int raceID = 2021;
                // Create race and vehicles
                PopulateDatabase(raceID);

                informationsService = new RaceInformationsServiceImpl(context);
                raceService = new RaceServiceImpl(context, new Mock<ILogger<RaceServiceImpl>>().Object);

                // Start the race
                raceService.StartRace(raceID);

                // Verify - heavy malfunction occured 100%
                var heavyMalfunction100 = context.Vehicles.Where(x => x.TeamName == "HeavyMalfunctionTeam100" && x.RaceId == raceID);
                Assert.AreEqual(10, heavyMalfunction100.Count());
                foreach (var vehicle in heavyMalfunction100)
                {
                    Assert.AreEqual(true, vehicle.IsHeavyMalfunctionOccured);
                    Assert.AreEqual(VehicleStatus.BREAKDOWN, vehicle.VehicleStatus);
                }

                // Verify - all vehicles with heavy malfunction
                var allWitHeavyMalfunction = context.Vehicles.Where(x => x.IsHeavyMalfunctionOccured == true && x.RaceId == raceID);
                Assert.GreaterOrEqual(allWitHeavyMalfunction.Count(), 10);
                foreach (var vehicle in allWitHeavyMalfunction)
                {
                    Assert.AreEqual(VehicleStatus.BREAKDOWN, vehicle.VehicleStatus);
                }

                // Verify - heavy malfunction occured 0%
                var heavyMalfunction0 = context.Vehicles.Where(x => x.TeamName == "HeavyMalfunctionTeam0" && x.RaceId == raceID);
                Assert.AreEqual(5, heavyMalfunction0.Count());
                foreach (var vehicle in heavyMalfunction0)
                {
                    Assert.AreEqual(false, vehicle.IsHeavyMalfunctionOccured);
                    Assert.AreEqual(VehicleStatus.FINISHRACE, vehicle.VehicleStatus);
                }

                // Verify - all vehicles without heavy malfunction
                var allWithoutHeavyMalfunction = context.Vehicles.Where(x => x.IsHeavyMalfunctionOccured == false && x.RaceId == raceID);
                Assert.GreaterOrEqual(allWithoutHeavyMalfunction.Count(), 5);
                foreach(var vehicle in allWithoutHeavyMalfunction)
                {
                    Assert.AreEqual(VehicleStatus.FINISHRACE, vehicle.VehicleStatus);
                }
            }
        }

        [Test]
        public void RaceInformationsWhenRaceIsInRunningState_Test()
        {
            int raceId = 2024;
            // Create new race
            PopulateDatabase(raceId);

            // Start the race
            Thread startRaceThread = new Thread(() => StartRace(raceId));
            startRaceThread.Start();

            using (var context = new VehicleDbContext(options))
            {
                SpinWait.SpinUntil(() => { Thread.Sleep(1000); return context.Races.Where(x => x.Year == raceId).FirstOrDefault().State == RaceState.RUNNING; }, 5000);

                Assert.AreEqual(RaceState.RUNNING, context.Races.Where(x => x.Year == raceId).FirstOrDefault().State);

                informationsService = new RaceInformationsServiceImpl(context);
                var leaderboardCars = informationsService.GetLeaderboardForVehicle("cars");
                var leaderboardAll = informationsService.GetLeaderboardForAllVehicles();
                Assert.IsNotNull(leaderboardCars);
                Assert.IsNotNull(leaderboardAll);

                syncRace.WaitOne();
                SpinWait.SpinUntil(() => { return context.Races.Where(x => x.Year == raceId).FirstOrDefault().State == RaceState.FINISHED; }, 5000);
            }
        }
        private void StartRace(int raceId)
        {
            using (var context = new VehicleDbContext(options))
            {
                raceService = new RaceServiceImpl(context, new Mock<ILogger<RaceServiceImpl>>().Object);
                // Start the race
                raceService.StartRace(raceId);
                syncRace.Set();
            }
        }
        private void CreateRace(VehicleDbContext context, int raceId)
        {
            context.Races.Add(new Race(raceId));
            context.SaveChanges();
        }
        private async Task AddVehicleToTheRace(VehicleDbContext context, Vehicle sc, int raceId)
        {      
            sc.RaceId = raceId;
            sc.TeamName = "Team300";
            sc.Type = "SPORTCAR";
            sc.VehicleModel = "model1";
            await raceService.AddVehicleToRace(sc, raceId);
            var v = context.Vehicles.Where(x => x.RaceId == raceId).FirstOrDefault();
            Assert.AreEqual(raceId, v.RaceId);
            Assert.AreEqual("SPORTCAR", v.Type);
            Assert.AreEqual("model1", v.VehicleModel);
            context.SaveChanges();
        }

        private async Task UpdateVehicleInfo(VehicleDbContext context, Vehicle sc, int raceId)
        {
            sc.Type = "updatedType";
            sc.VehicleModel = "updatedModel";
            await raceService.UpdateVehicleInfo(sc, sc.Id);
            var v = context.Vehicles.Where(x => x.RaceId == raceId).FirstOrDefault();
            Assert.AreEqual(raceId, v.RaceId);
            Assert.AreEqual("updatedType", v.Type);
            Assert.AreEqual("updatedModel", v.VehicleModel);
            context.SaveChanges();
        }

        private async Task DeleteVehicle(VehicleDbContext context, Vehicle sc, int raceId)
        {
            await raceService.DeleteVehicle(sc.Id);
            Assert.IsNull(context.Vehicles.Where(x => x.RaceId == raceId).FirstOrDefault());
            context.SaveChanges();
        }

        private void PopulateDatabase(int raceId)
        {
            using (var context = new VehicleDbContext(options))
            {
                context.Races.Add(new Race(raceId) { });

                List<Vehicle> vehicles = new List<Vehicle>(25);
                for (int i = 0; i < 10; i++)
                {
                    var sc = new SportCar();
                    sc.RaceId = raceId;
                    sc.TeamName = "HeavyMalfunctionTeam100";
                    sc.Type = "SPORTCAR";
                    sc.VehicleModel = "model1";
                    // Has heavy malfunction 100%
                    sc.HeavyMalfunctionProbability = 1.0;
                    vehicles.Add(sc);
                }
                for (int i = 0; i < 5; i++)
                {
                    var sc = new CrossMotorbike();
                    sc.RaceId = raceId;
                    sc.TeamName = "HeavyMalfunctionTeam0";
                    sc.Type = "CROSSMOTORBIKE";
                    sc.VehicleModel = "model1";
                    // Has heavy malfunction 0%
                    sc.HeavyMalfunctionProbability = 0.0;
                    vehicles.Add(sc);
                }
                for (int i = 0; i < 5; i++)
                {
                    var sc = new SportMotorbike();
                    sc.RaceId = raceId;
                    sc.TeamName = "Team" + i;
                    sc.Type = "SPORTMOTORBIKE";
                    sc.VehicleModel = "model1";
                    sc.VehicleStatus = VehicleStatus.NOSTATUS;
                    vehicles.Add(sc);
                }
                for (int i = 0; i < 5; i++)
                {
                    var sc = new TerrainCar();
                    sc.RaceId = raceId;
                    sc.TeamName = "Team" + i;
                    sc.Type = "TERRAINCAR";
                    sc.VehicleModel = "model1";
                    vehicles.Add(sc);
                }
                for (int i = 0; i < 5; i++)
                {
                    var sc = new Truck();
                    sc.RaceId = raceId;
                    sc.TeamName = "Team" + i;
                    sc.Type = "TRUCK";
                    sc.VehicleModel = "model1";
                    vehicles.Add(sc);
                }
                context.Vehicles.AddRange(vehicles);
                context.SaveChanges();
            }
        }
    }
}