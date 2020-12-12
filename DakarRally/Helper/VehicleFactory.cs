using DakarRally.Models;

namespace DakarRally.Helper
{
    public abstract class VehicleFactory
    {
        public abstract Vehicle GetVehicle(string jsonResult);

        protected static T DeserializeObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
            });
        }
    }
}
