using AutoMapper;
using Devices.Application.AutoMapper;
using FluentAssertions;
using Xunit;

namespace Devices.Application.Tests.Tests.AutoMapper
{
    public class AutoMapperProfileTests
    {
        [Fact]
        public void ProfileIsValid()
        {
            true.Should().BeFalse();
            // Arrange
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());

            // Act & Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
