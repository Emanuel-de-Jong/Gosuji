using Gosuji.Client.Helpers.HttpResponseHandler;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;

namespace Gosuji.API.Controllers.HubFilters
{
    public class ValidateHubFilter : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            string methodName = invocationContext.HubMethodName;
            Type hubType = invocationContext.Hub.GetType();
            IReadOnlyList<object?> hubMethodArguments = invocationContext.HubMethodArguments;

            MethodInfo? method = hubType.GetMethod(methodName);
            if (method == null)
            {
                return await next(invocationContext);
            }

            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < hubMethodArguments.Count; i++)
            {
                object? argument = hubMethodArguments[i];
                ParameterInfo parameter = parameters[i];

                if (argument == null)
                {
                    continue;
                }

                ValidationAttribute[] validationAttributes = (ValidationAttribute[])parameter.GetCustomAttributes(typeof(ValidationAttribute), false);
                foreach (ValidationAttribute attribute in validationAttributes)
                {
                    if (!attribute.IsValid(argument))
                    {
                        return new HubResponse(HttpStatusCode.BadRequest, attribute.FormatErrorMessage(parameter.Name));
                    }
                }

                Type argumentType = argument.GetType();
                if (!(argumentType.IsPrimitive || argument is string))
                {
                    ValidationContext context = new(argument);
                    List<ValidationResult> results = new(1);

                    if (!Validator.TryValidateObject(argument, context, results, true))
                    {
                        return new HubResponse(HttpStatusCode.BadRequest, string.Join(", ", results.Select(v => v.ErrorMessage)));
                    }
                }
            }

            return await next(invocationContext);
        }
    }
}
