using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DakarRally.Helper
{
    public class ValidateActionParametersAttribute : ActionFilterAttribute
    {
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

    public class ValidateTypeParameterAttribute : ActionFilterAttribute
    {
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

    public class ValidateOrderParameterAttribute : ActionFilterAttribute
    {
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