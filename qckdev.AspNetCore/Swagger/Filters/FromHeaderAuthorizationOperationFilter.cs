using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace qckdev.AspNetCore.Swagger.Filters
{
    /// <summary>
    /// Adds an Authorization header parameter when an action uses [FromHeader(Name = "Authorization")].
    /// </summary>
    public sealed class FromHeaderAuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null)
            {
                return;
            }

            var authorizationParameter = context.MethodInfo
                .GetParameters()
                .Select(x => new
                {
                    Parameter = x,
                    FromHeader = x.GetCustomAttribute<FromHeaderAttribute>()
                })
                .FirstOrDefault(x => x.FromHeader != null && string.Equals(
                    string.IsNullOrWhiteSpace(x.FromHeader.Name) ? x.Parameter.Name : x.FromHeader.Name,
                    "Authorization",
                    StringComparison.OrdinalIgnoreCase));

            if (authorizationParameter == null)
            {
                return;
            }

            operation.Parameters ??= new List<OpenApiParameter>();
            if (operation.Parameters.Any(x =>
                    x.In == ParameterLocation.Header
                    && string.Equals(x.Name, "Authorization", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var parameterName = authorizationParameter.Parameter.Name ?? "authorization";
            var description = ResolveDescription(context, parameterName);

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Required = true,
                Description = description,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }

        static string? ResolveDescription(OperationFilterContext context, string parameterName)
        {
            var description = context.ApiDescription.ParameterDescriptions
                .FirstOrDefault(x => string.Equals(x.Name, parameterName, StringComparison.OrdinalIgnoreCase))
                ?.ModelMetadata?
                .Description;

            if (!string.IsNullOrWhiteSpace(description))
            {
                return description.Trim();
            }

            return ResolveXmlParameterDescription(context.MethodInfo, parameterName);
        }

        static string? ResolveXmlParameterDescription(MethodInfo methodInfo, string parameterName)
        {
            var declaringAssembly = methodInfo.DeclaringType?.Assembly;
            if (declaringAssembly == null)
            {
                return null;
            }

            var xmlCommentsPath = Path.ChangeExtension(declaringAssembly.Location, ".xml");
            if (!File.Exists(xmlCommentsPath))
            {
                return null;
            }

            var xmlComments = XDocument.Load(xmlCommentsPath);
            var memberName = BuildXmlMethodMemberName(methodInfo);
            var paramDescription = xmlComments
                .Descendants("member")
                .FirstOrDefault(x => string.Equals((string?)x.Attribute("name"), memberName, StringComparison.Ordinal))
                ?.Elements("param")
                .FirstOrDefault(x => string.Equals((string?)x.Attribute("name"), parameterName, StringComparison.Ordinal))
                ?.Value;

            return string.IsNullOrWhiteSpace(paramDescription) ? null : paramDescription.Trim();
        }

        static string BuildXmlMethodMemberName(MethodInfo methodInfo)
        {
            var typeName = methodInfo.DeclaringType?.FullName?.Replace("+", ".") ?? string.Empty;
            var parameterTypes = methodInfo.GetParameters()
                .Select(x => GetXmlTypeName(x.ParameterType))
                .ToArray();

            var methodName = methodInfo.Name;
            if (methodInfo.IsGenericMethod)
            {
                methodName += $"``{methodInfo.GetGenericArguments().Length}";
            }

            return parameterTypes.Length == 0
                ? $"M:{typeName}.{methodName}"
                : $"M:{typeName}.{methodName}({string.Join(",", parameterTypes)})";
        }

        static string GetXmlTypeName(Type type)
        {
            if (type.IsByRef)
            {
                return $"{GetXmlTypeName(type.GetElementType()!)}@";
            }

            if (type.IsArray)
            {
                return $"{GetXmlTypeName(type.GetElementType()!)}[]";
            }

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                var baseName = (genericType.FullName ?? genericType.Name).Split('`')[0].Replace("+", ".");
                var arguments = string.Join(",", type.GetGenericArguments().Select(GetXmlTypeName));
                return $"{baseName}{{{arguments}}}";
            }

            if (type.IsGenericParameter)
            {
                return type.DeclaringMethod != null ? $"``{type.GenericParameterPosition}" : $"`{type.GenericParameterPosition}";
            }

            return (type.FullName ?? type.Name).Replace("+", ".");
        }
    }
}
