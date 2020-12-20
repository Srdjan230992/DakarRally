using DakarRally.Exceptions;
using DakarRally.Factory;
using DakarRally.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using static DakarRally.Helper.AppEnums;

namespace DakarRally.Helper
{
    /// <summary>
    /// VehicleTypeModelBinder class.
    /// </summary>
    public class VehicleTypeModelBinder : IModelBinder
    {
        #region Fields

        private const string TYPE = "Type";

        #endregion

        #region Public methods

        /// <summary>
        /// Binding contrete vehicle class model.
        /// </summary>
        /// <param name="bindingContext">Model binding context.</param>
        /// <returns>Binding result.</returns>
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
                var type = jObject?.SelectToken(TYPE);
                if (type != null)
                {
                    obj = GetVehicle(type.ToString(), json.Result);
                }
            }
            catch (JsonReaderException) { throw new Exception("Json string is not valid!"); }
            finally
            {
                if (obj == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    
                }
                bindingContext.Result = ModelBindingResult.Success(obj);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Extracts json string.
        /// </summary>
        /// <param name="actionContext">The action conteht.</param>
        /// <returns>Extracted json string.</returns>
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
            var vehicleType = (VehicleType)Enum.Parse(typeof(VehicleType), vehicle.ToUpperInvariant());

            switch (vehicleType)
            {
                case VehicleType.SPORTCAR:
                    factory = new SportCarFactory();
                    break;
                case VehicleType.TERRAINCAR:
                    factory = new TerrainCarFactory();
                    break;
                case VehicleType.SPORTMOTORBIKE:
                    factory = new SportMotorbikeFactory();
                    break;
                case VehicleType.CROSSMOTORBIKE:
                    factory = new CrossMotorbikeFactory();
                    break;
                case VehicleType.TRUCK:
                    factory = new TruckFactory();
                    break;
                default:
                    throw new InvalidStateException($"Not supported vehicle type: {vehicleType.ToString()}!");
            }

            return factory != null ? factory.GetVehicle(jsonResult) : throw new VehiclesNotFoundException("Vehicle id not found!");
        }

        #endregion
    }
}