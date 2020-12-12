using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DakarRally.Helper.AppHelper;

namespace DakarRally.Models
{
    public abstract class Vehicle
    {
        public long Id { get; set; }
        [Required]
        [Range(1970, 2050)]
        public long RaceId { get; set; }
        [Required]
        [StringLength(14, ErrorMessage = "Type length can't be more than 14.")]
        public string Type { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "TeamName length can't be more than 100.")]
        public string TeamName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "VehicleModel length can't be more than 100.")]
        public string VehicleModel { get; set; }
        public DateTime VehicleManufaturingDate { get; set; }
        [NotMapped]
        public int VehicleSpeed { get; set; } = 0;
        [NotMapped]
        public int LightMalfunctionDelay { get; set; } = 0;
        public int LightMalfunctionCounter { get; set; } = 0;
        public int LightMalfunctionCount { get; set; } = 0;
        public int PassedDistance { get; set; } = 0;
        public bool VehicleFinishedRace { get; set; } = false;
        public bool IsLightMalfunctionOccured { get; set; } = false;
        public bool IsHeavyMalfunctionOccured { get; set; } = false;
        [NotMapped]
        public double LightMalfunctionProbability { get; set; } = 0;
        [NotMapped]
        public double HeavyMalfunctionProbability { get; set; } = 0;

        public int Rank { get; set; } = 0;
        public VehicleStatus VehicleStatus { get; set; } = VehicleStatus.NoStatus;

        protected int CalculateVehicleSpeed(int speed)
        {
             return (90 / 100) * speed; // 90 %
        }
    }
}
