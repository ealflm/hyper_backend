using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismSmartTransportation.API.Utilities.Swagger
{
    public class CustomSwaggerFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc.Info.Version.Equals("admin"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                .Where(x => !x.Key.ToLower().Contains("/admin/"))
                .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }
            if (swaggerDoc.Info.Version.Equals("partner"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                .Where(x => !x.Key.ToLower().Contains("/partner/"))
                .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }
            if (swaggerDoc.Info.Version.Equals("driver"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                .Where(x => !x.Key.ToLower().Contains("/driver/"))
                .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }
            if (swaggerDoc.Info.Version.Equals("customer"))
            {
                var nonMobileRoutes = swaggerDoc.Paths
                .Where(x => !x.Key.ToLower().Contains("/customer/"))
                .ToList();
                nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
            }
        }
    }
}
