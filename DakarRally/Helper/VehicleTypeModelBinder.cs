using DakarRally.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DakarRally.Helper
{
    /// <summary>
    /// VehicleTypeModelBinder class.
    /// </summary>
    public class VehicleTypeModelBinder : IModelBinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var json = ExtractRequestJson(bindingContext.ActionContext);
            Vehicle obj = null;
            try
            {
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(json.Result);
                var type = jObject?.SelectToken("Type");
                if (type != null)
                {
                    obj = GetVehicle(type.ToString(), json.Result);
                }
            }
            catch (JsonReaderException) { throw;  }
            finally
            {
                if (obj != null)
                {
                    bindingContext.Result = ModelBindingResult.Success(obj);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
        
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static Task<string> ExtractRequestJson(ActionContext actionContext)
        {
            var content = actionContext.HttpContext.Request.Body;
            return new StreamReader(content).ReadToEndAsync();
        }

        /// <summary>
        /// Determines which vehicle type will be created.
        /// </summary>
        /// <param name="vehicle">Desired vehicle.</param>
        /// <param name="jsonResult">Json result for deserialization.</param>
        /// <returns>Concrete vehicle.</returns>
        private Vehicle GetVehicle(string vehicle, string jsonResult)
        {
            VehicleFactory factory = null;

            switch (vehicle.ToLower())
            {
                case "sportcar":
                    factory = new SportCarFactory();
                    break;
                case "terraincar":
                    factory = new TerrainCarFactory();
                    break;
                case "sportmotorbike":
                    factory = new SportMotorbikeFactory();
                    break;
                case "crossmotorbike":
                    factory = new CrossMotorbikeFactory();
                    break;
                case "truck":
                    factory = new TruckFactory();
                    break;
                default:
                    break;
            }

            return factory != null ? factory.GetVehicle(jsonResult) : null;
        }
    }
}
