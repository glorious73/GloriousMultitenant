using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Configuration
{
    public class HeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            // Responses
            operation.Responses.Add("201", new OpenApiResponse { Description = "Created" });
            operation.Responses.Add("400", new OpenApiResponse { Description = "Bad Request" });
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            operation.Responses.Add("500", new OpenApiResponse { Description = "Internal Server Error" });
        }
    }
}
