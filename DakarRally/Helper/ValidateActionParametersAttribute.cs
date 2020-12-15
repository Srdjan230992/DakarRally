using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DakarRally.Helper
{
    /// <summary>
    /// ValidateActionParametersAttribute class.
    /// </summary>
    public class ValidateActionParametersAttribute : ActionFilterAttribute
    {
        /// <inheritdoc/>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor != null)
            {
                var parameters = descriptor.MethodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    var argument = context.ActionArguments[parameter.Name];

                    EvaluateValidationAttributes(parameter, argument, context.ModelState);
                }
            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Evaluates validation attributes.
        /// </summary>
        /// <param name="parameter">Discovers the attributes of a parameter and provides access to parameter metadata.</param>
        /// <param name="argument">Validation informations.</param>
        /// <param name="modelState">Model state.</param>
        private void EvaluateValidationAttributes(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
        {
            var validationAttributes = parameter.CustomAttributes;

            foreach (var attributeData in validationAttributes)
            {
                var attributeInstance = CustomAttributeExtensions.GetCustomAttribute(parameter, attributeData.AttributeType);

                var validationAttribute = attributeInstance as ValidationAttribute;

                if (validationAttribute != null)
                {
                    var isValid = validationAttribute.IsValid(argument);
                    if (!isValid)
                    {
                        modelState.AddModelError(parameter.Name, validationAttribute.FormatErrorMessage(parameter.Name));
                    }
                }
            }
        }
    }

    /// <summary>
    /// ValidateTypeParameterAttribute class.
    /// </summary>
    public class ValidateTypeParameterAttribute : ActionFilterAttribute
    {
        /// <inheritdoc/>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var types = new List<string>(3) { "cars", "trucks", "motorcycles" };
            if (filterContext.ActionArguments.ContainsKey("type"))
            {
                string name = filterContext.ActionArguments["type"] as string;

                if (name != null && !types.Contains(name))
                {
                    filterContext.Result = new BadRequestObjectResult("The type must be cars, trucks or motorcycles!");
                }
            }
        }
    }

    /// <summary>
    /// ValidateOrderParameterAttribute class.
    /// </summary>
    public class ValidateOrderParameterAttribute : ActionFilterAttribute
    {
        /// <inheritdoc/>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var types = new List<string>(3) { "asc", "desc" };
            if (filterContext.ActionArguments.ContainsKey("order"))
            {
                string name = filterContext.ActionArguments["order"] as string;

                if (name != null && !types.Contains(name))
                {
                    filterContext.Result = new BadRequestObjectResult("The order must be asc or desc!");
                }
            }
        }
    }
}