using Bogus;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Fafnir.Throttle.NetCore.Tests
{
    [ExcludeFromCodeCoverage]
    public class ThrottleServiceTests
    {
        private readonly Mock<IOptions<ThrottleConfiguration>> throttleOptionConfigurationMock = new Mock<IOptions<ThrottleConfiguration>>();
        private readonly Faker faker = new Faker();

        [Fact]
        public void NewClient_NotExceeded_IsAllowed()
        {
            //Arrange
            throttleOptionConfigurationMock.Setup(x => x.Value)
                .Returns(new ThrottleConfiguration
                {
                    MaxRequests = 5,
                    PenaltyTime = TimeSpan.FromSeconds(5),
                    Period = TimeSpan.FromSeconds(1),
                    ErrorMessage = "Too many requests."
                });

            var memoryOptionsMock = new Mock<IOptions<MemoryCacheOptions>>();
            memoryOptionsMock.Setup(x => x.Value)
                .Returns(new MemoryCacheOptions());

            var throttleService = new ThrottleService(new MemoryCache(memoryOptionsMock.Object), throttleOptionConfigurationMock.Object);

            //Act
            var ip = faker.Internet.Ip();

            for (int i = 0; i < 4; i++)
            {
                throttleService.IsAllowed(ip);
            }

            //Assert
            Assert.True(throttleService.IsAllowed(ip));
        }

        [Fact]
        public void Client_WhenExceeds_IsBan()
        {
            //Arrange
            throttleOptionConfigurationMock.Setup(x => x.Value)
                .Returns(new ThrottleConfiguration
                {
                    MaxRequests = 5,
                    PenaltyTime = TimeSpan.FromSeconds(5),
                    Period = TimeSpan.FromSeconds(1),
                    ErrorMessage = "Too many requests."
                });

            var memoryOptionsMock = new Mock<IOptions<MemoryCacheOptions>>();
            memoryOptionsMock.Setup(x => x.Value)
                .Returns(new MemoryCacheOptions());

            var throttleService = new ThrottleService(new MemoryCache(memoryOptionsMock.Object), throttleOptionConfigurationMock.Object);

            //Act
            var ip = faker.Internet.Ip();

            for (int i = 0; i < 10; i++)
            {
                throttleService.IsAllowed(ip);
            }

            //Assert
            Assert.False(throttleService.IsAllowed(ip));
        }

        [Fact]
        public void BannedClient_AfterExceeded_CanRequest()
        {
            //Arrange
            throttleOptionConfigurationMock.Setup(x => x.Value)
                .Returns(new ThrottleConfiguration
                {
                    MaxRequests = 5,
                    PenaltyTime = TimeSpan.FromSeconds(5),
                    Period = TimeSpan.FromSeconds(1),
                    ErrorMessage = "Too many requests."
                });

            var memoryOptionsMock = new Mock<IOptions<MemoryCacheOptions>>();
            memoryOptionsMock.Setup(x => x.Value)
                .Returns(new MemoryCacheOptions());

            var throttleService = new ThrottleService(new MemoryCache(memoryOptionsMock.Object), throttleOptionConfigurationMock.Object);

            //Act
            var ip = faker.Internet.Ip();

            for (int i = 0; i < 10; i++)
            {
                throttleService.IsAllowed(ip);
            }

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(6));

            //Assert
            Assert.True(throttleService.IsAllowed(ip));
        }
    }
}
