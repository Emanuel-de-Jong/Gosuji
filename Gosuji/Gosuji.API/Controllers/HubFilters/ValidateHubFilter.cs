using Gosuji.Client.Helpers.HttpResponseHandler;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Gosuji.API.Controllers.HubFilters
{
    public class ValidateHubFilter : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            foreach (object? argument in invocationContext.HubMethodArguments)
            {
                if (argument == null)
                {
                    continue;
                }

                ValidationContext context = new(argument);
                List<ValidationResult> results = new();
                if (!Validator.TryValidateObject(argument, context, results, true))
                {
                    string errorMessage = string.Join(", ", results.Select(v => v.ErrorMessage));
                    return new HubResponse(HttpStatusCode.BadRequest, errorMessage);
                }
            }

            return await next(invocationContext);

        }
    }
}
