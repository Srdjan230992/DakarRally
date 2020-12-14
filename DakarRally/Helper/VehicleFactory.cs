using DakarRally.Models;

namespace DakarRally.Helper
{
    /// <summary>
    /// VehicleFactory class.
    /// </summary>
    public abstract class VehicleFactory
    {
        /// <summary>
        /// Retrives object for concrete vehicle type.
        /// </summary>
        /// <param name="jsonResult">Json object for concrete vehicle class deserialization.</param>
        /// <returns>Object for concrete vehicle type.</returns>
        public abstract Vehicle GetVehicle(string jsonResult);

        /// <summary>
        /// Deserializes concrete vehicle type.
        /// </summary>
        /// <typeparam name="T">Concrete vehicle type.</typeparam>
        /// <param name="json">Json object for concrete vehicle class deserialization.</param>
        /// <returns>Object for concrete vehicle type.</returns>
        protected static T DeserializeObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
            });
        }
    }
}
