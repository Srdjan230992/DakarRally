using DakarRally.Helper;
using DakarRally.Models;
using DakarRally.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using static DakarRally.Helper.AppEnums;

namespace DakarRallyTests
{
    public class DakarRallyTests
    {
        private RaceInformationsServiceImpl service;
        private DbContextOptions<VehicleDbContext> options;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<VehicleDbContext>().UseInMemoryDatabase(databaseName: "Vehicles").Options;
            PopulateDatabase();
        }

        [Test]
        public void VehicleStatistic_Test()
        {
            using (var context = new VehicleDbContext(options))
            {
                service = new RaceInformationsServiceImpl(context);
                VehicleStatistic vs = service.GetVehicleStatistics(2);

                Assert.AreEqual(0, vs.Distance);
                Assert.AreEqual(false, vs.IsHeayMalfanctionOssured);
                Assert.AreEqual(0, vs.LightMulfunctionCount);
            }
        }

        private void PopulateDatabase()
        {
            using (var context = new VehicleDbContext(options))
            {
                context.Races.Add(new Race(2020) { });

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
                context.Vehicles.AddRange(vehicles);
                context.SaveChanges();
            }
        }
    }
}