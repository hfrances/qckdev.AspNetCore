using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using qckdev.AspNetCore.FluentValidation;
using System;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Test.Integration
{
    [TestClass]
    public class FluentValidationOptionsIntegrationTests
    {
        [TestMethod]
        public void ValidateFluentValidation_WithInvalidOptions_ThrowsOptionsValidationException()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IValidator<TestSettings>, TestSettingsValidator>();
                    services.AddOptions<TestSettings>()
                        .Configure(options => options.Connection = string.Empty)
                        .ValidateFluentValidation();
                });
            using var host = hostBuilder.Build();
            var options = host.Services.GetRequiredService<IOptions<TestSettings>>();

            // Act
            Action action = () => { _ = options.Value; };

            // Assert
            var exception = action.Should().Throw<OptionsValidationException>().Which;
            exception.Failures.Should().Contain(failure => failure.Contains("Connection"));
        }

        [TestMethod]
        public void ValidateFluentValidation_WithNamedOptions_ValidatesOnlyRequestedName()
        {
            // Arrange
            const string validName = "valid";
            const string invalidName = "invalid";
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IValidator<TestSettings>, TestSettingsValidator>();
                    services.AddOptions<TestSettings>(validName)
                        .Configure(options => options.Connection = "https://api.example.net")
                        .ValidateFluentValidation();
                    services.AddOptions<TestSettings>(invalidName)
                        .Configure(options => options.Connection = string.Empty)
                        .ValidateFluentValidation();
                });
            using var host = hostBuilder.Build();
            var monitor = host.Services.GetRequiredService<IOptionsMonitor<TestSettings>>();

            // Act
            Action validAction = () => { _ = monitor.Get(validName); };
            Action invalidAction = () => { _ = monitor.Get(invalidName); };

            // Assert
            validAction.Should().NotThrow();
            invalidAction.Should().Throw<OptionsValidationException>();
        }

        [TestMethod]
        public async Task ValidateOnStartCompat_WithInvalidOptions_FailsHostStartup()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IValidator<TestSettings>, TestSettingsValidator>();
                    services.AddOptions<TestSettings>()
                        .Configure(options => options.Connection = string.Empty)
                        .ValidateFluentValidation()
                        .ValidateOnStartCompat();
                });

            using var host = hostBuilder.Build();

            // Act
            Func<Task> action = async () => await host.StartAsync();

            // Assert
            await action.Should().ThrowAsync<OptionsValidationException>();
        }

        [TestMethod]
        public async Task ValidateOnStartCompat_WithValidOptions_StartsHost()
        {
            // Arrange
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IValidator<TestSettings>, TestSettingsValidator>();
                    services.AddOptions<TestSettings>()
                        .Configure(options => options.Connection = "https://api.example.net")
                        .ValidateFluentValidation()
                        .ValidateOnStartCompat();
                });

            using var host = hostBuilder.Build();

            // Act
            Func<Task> action = async () => await host.StartAsync();

            // Assert
            await action.Should().NotThrowAsync();
            await host.StopAsync();
        }

        private sealed class TestSettings
        {
            public string Connection { get; set; } = string.Empty;
        }

        private sealed class TestSettingsValidator : AbstractValidator<TestSettings>
        {
            public TestSettingsValidator()
            {
                RuleFor(options => options.Connection).NotEmpty();
            }
        }
    }
}
