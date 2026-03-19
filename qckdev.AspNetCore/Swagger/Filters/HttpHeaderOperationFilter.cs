using qckdev.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qckdev.AspNetCore.Swagger.Filters
{
    /// <summary>
    /// Adds OpenAPI header parameters from endpoint attributes that implement <see cref="IHttpHeaderAttribute" />.
    /// </summary>
    public sealed class HttpHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            IEnumerable<IFilterMetadata> globalAttributes = context
                .ApiDescription
                .ActionDescriptor
                .FilterDescriptors
                .Select(x => x.Filter);

            object[] controllerAttributes = context
                .MethodInfo
                .DeclaringType?
                .GetCustomAttributes(true) ?? Array.Empty<object>();

            object[] methodAttributes = context
                .MethodInfo
                .GetCustomAttributes(true);

            IEnumerable<IHttpHeaderAttribute> headerAttributes = globalAttributes
                .Union(controllerAttributes)
                .Union(methodAttributes)
                .OfType<IHttpHeaderAttribute>()
                .GroupBy(x => x.GetType())
                .Select(x => x.Last()) // The last one is the most specific.
                .Where(x => x.IsAvailable);

            foreach (IHttpHeaderAttribute headerAttribute in headerAttributes.Reverse())
            {
                operation.Parameters.Insert(0, new OpenApiParameter
                {
                    Name = headerAttribute.HeaderName,
                    In = ParameterLocation.Header,
                    Required = headerAttribute.IsMandatory,
                    Schema = new OpenApiSchema { Type = "string" }
                });
            }
        }
    }
}
