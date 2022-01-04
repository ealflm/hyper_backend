using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Text.RegularExpressions;

namespace TourismSmartTransportation.API.Utilities.Swagger
{
    public class KebabCaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.ToDictionary(
                entry => string.Join('/', entry.Key.Split('/').Select(x => x.ToLower())),
                entry => entry.Value);

            swaggerDoc.Paths = new OpenApiPaths();

            foreach ((string key, OpenApiPathItem value) in paths)
            {
                foreach (OpenApiParameter param in value.Operations.SelectMany(o => o.Value.Parameters))
                {
                    param.Name = ToKebabCase(param.Name);
                }

                swaggerDoc.Paths.Add(key, value);
            }
        }

        private static string ToKebabCase(object value)
        {
            return value == null ? null : Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}
