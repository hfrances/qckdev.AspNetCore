using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using qckdev.AspNetCore.Services;

namespace qckdev.AspNetCore.Test.Unit
{
    [TestClass]
    public class HostEnvironmentServiceTests
    {
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock = null!;
        private HostEnvironmentService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        }

        [TestMethod]
        public void IsDevelopment_WithDevelopmentEnvironment_ReturnsTrue()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns("Development");
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act
            var result = _service.IsDevelopment();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsDevelopment_WithProductionEnvironment_ReturnsFalse()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns("Production");
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act
            var result = _service.IsDevelopment();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProduction_WithProductionEnvironment_ReturnsTrue()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns("Production");
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act
            var result = _service.IsProduction();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsProduction_WithDevelopmentEnvironment_ReturnsFalse()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns("Development");
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act
            var result = _service.IsProduction();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsStaging_WithStagingEnvironment_ReturnsTrue()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns("Staging");
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act
            var result = _service.IsStaging();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsDocker_WithDockerEnvironment_ReturnsTrue()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns("Docker");
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act
            var result = _service.IsDocker();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsDocker_WithNonDockerEnvironment_ReturnsFalse()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns("Production");
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act
            var result = _service.IsDocker();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void EnvironmentMethods_WithNullEnvironmentName_HandlesGracefully()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns((string?)null);
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act & Assert - Should not throw
            _service.IsDevelopment().Should().BeFalse();
            _service.IsProduction().Should().BeFalse();
            _service.IsStaging().Should().BeFalse();
            _service.IsDocker().Should().BeFalse();
        }

        [TestMethod]
        public void EnvironmentMethods_WithEmptyEnvironmentName_HandlesGracefully()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns(string.Empty);
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act & Assert - Should not throw
            _service.IsDevelopment().Should().BeFalse();
            _service.IsProduction().Should().BeFalse();
        }

        [TestMethod]
        public void EnvironmentMethods_AreCaseInsensitive()
        {
            // Arrange
            _webHostEnvironmentMock.Setup(h => h.EnvironmentName).Returns("DEVELOPMENT");
            _service = new HostEnvironmentService(_webHostEnvironmentMock.Object);

            // Act
            var result = _service.IsDevelopment();

            // Assert
            result.Should().BeTrue("Environment comparison should be case-insensitive");
        }
    }
}
