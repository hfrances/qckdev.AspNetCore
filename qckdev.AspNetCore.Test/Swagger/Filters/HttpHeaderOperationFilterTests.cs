using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swashbuckle.AspNetCore.SwaggerGen;
using qckdev.AspNetCore.Swagger.Filters;
using qckdev.AspNetCore.Test.Fixtures;
using System;
using System.Linq;

namespace qckdev.AspNetCore.Test.Swagger.Filters
{
    [TestClass]
    public class HttpHeaderOperationFilterTests
    {
        [TestMethod]
        public void Apply_AddsHeaderParameter_WhenAttributePresent()
        {
            // Arrange
            var operation = new OpenApiOperation();
            var method = typeof(HttpTestFlagHeaderTestController).GetMethod("GetWithMandatoryTestFlag");
            var apiDescription = new ApiDescription
            {
                ActionDescriptor = new ControllerActionDescriptor { MethodInfo = method! }
            };

            var context = new OperationFilterContext(apiDescription, null!, null!, method!);
            var filter = new HttpHeaderOperationFilter();

            // Act
            filter.Apply(operation, context);

            // Assert
            operation.Parameters.Should().Contain(p =>
                string.Equals(p.Name, HttpTestFlagHeaderAttribute.HeaderNameValue, StringComparison.OrdinalIgnoreCase)
                && p.In == ParameterLocation.Header
                && p.Required);
        }
    }
}
