using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DakarRally.Helper.AppEnums;

namespace DakarRally.Models
{
    /// <summary>
    /// Vehicle class.
    /// </summary>
    public abstract class Vehicle
    {
        /// <summary>
        /// Vehicle identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Race id.
        /// </summary>
        [Required]
        [Range(1970, 2050)]
        public long RaceId { get; set; }

        /// <summary>
        /// Vehicle type.
        /// </summary>
        [Required]
        [StringLength(14, ErrorMessage = "Type length can't be more than 14.")]
        public string Type { get; set; }

        /// <summary>
        /// Team name.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "TeamName length can't be more than 100.")]
        public string TeamName { get; set; }

        /// <summary>
        /// Vehicle model.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "VehicleModel length can't be more than 100.")]
        public string VehicleModel { get; set; }

        /// <summary>
        /// Vehicle manufacturing date.
        /// </summary>
        public DateTime VehicleManufaturingDate { get; set; }

        /// <summary>
        /// Vehicle speed.
        /// </summary>
        [NotMapped]
        public int VehicleSpeed { get; set; } = 0;

        /// <summary>
        /// Light malfunction delay.
        /// </summary>
        [NotMapped]
        public int LightMalfunctionDelay { get; set; } = 0;

        /// <summary>
        /// Light malfunction counter.
        /// </summary>
        public int LightMalfunctionCounter { get; set; } = 0;

        /// <summary>
        /// Light malfunction count (counting how much vehicle needs to be in malfunction state).
        /// </summary>
        public int LightMalfunctionCount { get; set; } = 0;

        /// <summary>
        /// Vehicle passed distance.
        /// </summary>
        public int PassedDistance { get; set; } = 0;

        /// <summary>
        /// Vehicle finished race.
        /// </summary>
        public bool VehicleFinishedRace { get; set; } = false;

        /// <summary>
        /// Is light malfunction occured.
        /// </summary>
        public bool IsLightMalfunctionOccured { get; set; } = false;

        /// <summary>
        /// Is heavy malfunction occured.
        /// </summary>
        public bool IsHeavyMalfunctionOccured { get; set; } = false;

        /// <summary>
        /// Light malfunction probability.
        /// </summary>
        [NotMapped]
        public double LightMalfunctionProbability { get; set; } = 0;

        /// <summary>
        /// Heavy malfunction probability
        /// </summary>
       // [NotMapped]
        public double HeavyMalfunctionProbability { get; set; } = 0;

        /// <summary>
        /// Rank in race.
        /// </summary>
        public int Rank { get; set; } = 0;

        /// <summary>
        /// Vehicle status.
        /// </summary>
        public VehicleStatus VehicleStatus { get; set; } = VehicleStatus.NoStatus;
    }
}