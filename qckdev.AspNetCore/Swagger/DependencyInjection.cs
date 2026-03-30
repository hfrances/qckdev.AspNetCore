using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Reflection;

namespace qckdev.AspNetCore.Swagger
{
    /// <summary>
    /// Extension methods for Swagger/OpenAPI services.
    /// </summary>
    public static class DependencyInjection
    {

        const string SWAGGER_V1_ID = "v1";
        const string SWAGGER_V1_NAME = "Default v1";

        /// <summary>
        /// Adds Swagger/OpenAPI services to the dependency injection container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="setupAction">Optional setup action for SwaggerGen options.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, Action<SwaggerGenOptions>? setupAction = null)
        {
            var assembly = Assembly.GetCallingAssembly();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SWAGGER_V1_ID, new OpenApiInfo
                {
                    Version = SWAGGER_V1_ID,
                    Title = $"{assembly.GetName().Name}"
                });
                c.DocumentFilter<Filters.DescriptionDocumentFilter>(assembly);
                c.OperationFilter<Filters.CustomAttributeFilter>();
                c.CustomSchemaIds(x => x.FullName?.Replace("+", "."));
                c.IncludeXmlComments(assembly);
                setupAction?.Invoke(c);
            });
            return services;
        }

        /// <summary>
        /// Adds Bearer token security definition to Swagger.
        /// </summary>
        /// <param name="c">The SwaggerGen options.</param>
        /// <param name="scheme">The security scheme name (e.g., "Bearer").</param>
        public static void AddSecurityBearer(this SwaggerGenOptions c, string scheme)
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = scheme,
                Description = $"Authorization header using the Bearer scheme '{scheme}'.<br>" +
                              "Enter <b>ONLY</b> your token in the text input (without the prefix 'Bearer').",
                Reference = new OpenApiReference
                {
                    Id = scheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            c.AddSecurityRequirementsOperationFilter();
        }

        /// <summary>
        /// Adds an operation filter that maps ASP.NET authorization metadata to OpenAPI security requirements.
        /// </summary>
        /// <param name="c">The SwaggerGen options.</param>
        public static void AddSecurityRequirementsOperationFilter(this SwaggerGenOptions c)
        {
            c.OperationFilter<Filters.SecurityRequirementsOperationFilter>();
        }

        /// <summary>
        /// Enables Swagger UI in the application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
            => UseSwagger(app, null);

        /// <summary>
        /// Enables Swagger UI in the application with custom setup.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="setupAction">Optional setup action for SwaggerUI options.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, Action<SwaggerUIOptions>? setupAction = null)
        {
            var assembly = Assembly.GetCallingAssembly();

            SwaggerBuilderExtensions.UseSwagger(app, setupAction: null);
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.DocumentTitle = $"{assembly.GetName().Name} - {c.DocumentTitle}";
                c.SwaggerEndpoint($"{SWAGGER_V1_ID}/swagger.json", SWAGGER_V1_NAME);
                c.DocExpansion(DocExpansion.List); // Endpoints listed.
                c.DefaultModelsExpandDepth(0); // Schema collapsed.
                setupAction?.Invoke(c);
            });

            return app;
        }

        /// <summary>
        /// Sets the comments path for the Swagger JSON and UI from assembly XML documentation.
        /// </summary>
        private static void IncludeXmlComments(this SwaggerGenOptions options, Assembly assembly)
        {
            var xmlFile = System.IO.Path.ChangeExtension(assembly.Location, ".xml");
            if (System.IO.File.Exists(xmlFile))
            {
                options.IncludeXmlComments(xmlFile);
            }
        }

    }
}
