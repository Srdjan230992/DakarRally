using System;
using System.Collections.Generic;

namespace DakarRally.Helper
{
    public class FilterData
    {
        public string Team { get; set; }
        public FilterItem Model { get; set; }
        public FilterItem ManufacturingDate { get; set; }
        public FilterItem Status { get; set; }
        public FilterItem Distance { get; set; }
    }
}
