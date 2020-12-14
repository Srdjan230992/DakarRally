using System.ComponentModel.DataAnnotations;
using static DakarRally.Helper.AppEnums;

namespace DakarRally.Models
{
    /// <summary>
    /// Race class.
    /// </summary>
    public class Race
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Race"/> class.
        /// </summary>
        /// <param name="year">Race year.</param>
        public Race(int year)
        {
            Year = year;
            State = RaceState.Pending;
        }

        /// <summary>
        /// Race identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Race year.
        /// </summary>
        [Required]
        public int Year { get; set; }

        /// <summary>
        /// Race state.
        /// </summary>
        public RaceState State { get; set; }
    }
}
