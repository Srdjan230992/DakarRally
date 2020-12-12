using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using static DakarRally.Helper.AppHelper;

namespace DakarRally.Helper
{
    public class FilterItem
    {
        public string Field { get; set; }
        public string LogicOperation { get; set; }
    }
}
