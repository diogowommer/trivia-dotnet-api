using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TriviaDotNetApi.API
{

    /// <summary>
    /// Esta classe é um filtro para retirar da obrigação de informar api-filter do swagger
    /// </summary>
    public class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "api-version");
            operation.Parameters.Remove(versionParameter);
        }
    }
}