using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static DakarRally.Helper.AppHelper;

namespace DakarRally.Models
{
    public class Race
    {
        public Race(int year)
        {
            Year = year;
            State = RaceState.Pending;
        }
        public long Id { get; set; }

        [Required]
        public int Year { get; set; }

        public RaceState State { get; set; }
    }
}
