using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mobi.Web.Utilities.Filters
{
    public class RemoveUnauthorizedRedirectFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var unauthorizedResponse = operation.Responses.FirstOrDefault(r => r.Key == "401");
            if (!unauthorizedResponse.Equals(default(KeyValuePair<string, OpenApiResponse>)))
            {
                operation.Responses.Remove("401");
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "Unauthorized - Invalid or missing JWT token."
                });
            }
        }
    }
}
