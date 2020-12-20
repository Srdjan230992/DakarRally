using DakarRally.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using static DakarRally.Helper.AppEnums;

namespace DakarRally
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            var options = new DbContextOptionsBuilder<VehicleDbContext>().UseInMemoryDatabase(databaseName: "Vehicles").Options;
            using (var context = new VehicleDbContext(options))
            {
                Race race = new Race(2020);
                race.Id = 1;
                context.Races.Add(race);

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
                    sc.VehicleStatus = VehicleStatus.NOSTATUS;
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
                context.SaveChangesAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}