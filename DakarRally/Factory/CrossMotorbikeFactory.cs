﻿using DakarRally.Models;

namespace DakarRally.Factory
{
    /// <summary>
    /// CrossMotorbikeFactory class.
    /// </summary>
    public class CrossMotorbikeFactory : VehicleFactory
    {
        /// <inheritdoc/>
        public override Vehicle GetVehicle(string jsonResult)
        {
            return DeserializeObject<CrossMotorbike>(jsonResult);
        }
    }
}
