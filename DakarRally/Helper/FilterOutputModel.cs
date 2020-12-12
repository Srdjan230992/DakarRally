using DakarRally.Models;
using System.Collections.Generic;

namespace DakarRally.Helper
{
    public class FilterOutputModel
    {
        public int? Count { get; set; }
        public IEnumerable<Vehicle> Items { get; set; }
    }
}
