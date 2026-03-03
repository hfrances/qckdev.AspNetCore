using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using qckdev.AspNetCore.Http.Metadata;
using qckdev.AspNetCore.Mvc.Headers;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qckdev.AspNetCore.Swagger.Filters
{
    /// <summary>
    /// Operation filter that adds custom HTTP header parameters from attributes.
    /// </summary>
    sealed class CustomAttributeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            IEnumerable<IFilterMetadata> globalAttributes = context
                .ApiDescription
                .ActionDescriptor
                .FilterDescriptors
                .Select(p => p.Filter);

            object[] controllerAttributes = context
               .MethodInfo
               .DeclaringType?
               .GetCustomAttributes(true) ?? Array.Empty<object>();

            object[] methodAttributes = context
                .MethodInfo
                .GetCustomAttributes(true);

            IEnumerable<IHttpHeaderAttribute> containsHeaderAttributes = globalAttributes
                .Union(controllerAttributes)
                .Union(methodAttributes)
                .OfType<IHttpHeaderAttribute>();

            IEnumerable<OpenApiParameter> httpHeaderParameters = containsHeaderAttributes
                .GroupBy(x => x.GetType())
                .Select(x => x.Last()) // The last one is the most specific.
                .Where(x => x.IsAvailable)
                .Select(x => new OpenApiParameter()
                {
                    Name = x.HeaderName,
                    In = ParameterLocation.Header,
                    Required = x.IsMandatory,
                    Schema = new OpenApiSchema() { Type = "string" }
                });

            foreach (var param in httpHeaderParameters.Reverse())
            {
                operation.Parameters.Insert(0, param);
            }
        }

    }
}
