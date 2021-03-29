using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Fafnir.Throttle.NetCore.Tests
{
    [ExcludeFromCodeCoverage]
    public class ThrottleServiceMiddlewareTests
    {
        private readonly Mock<IThrottleService> throttleServiceMock = new Mock<IThrottleService>();
        private readonly Mock<IOptions<ThrottleConfiguration>> throttleConfigurationOptionsMock = new Mock<IOptions<ThrottleConfiguration>>();
        private readonly Faker faker = new Faker();

        [Fact]
        public async Task Middleware_ShouldReturnOk_WhenRequestIsAllowed()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Request.Path = "/";
            defaultContext.Connection.RemoteIpAddress = faker.Internet.IpAddress();

            throttleServiceMock
                .Setup(x => x.IsAllowed(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            throttleConfigurationOptionsMock
                .SetupGet(x => x.Value)
                .Returns(new ThrottleConfiguration
                {
                    ErrorMessage = "Testing throttling.",
                    MaxRequests = 5,
                    PenaltyTime = TimeSpan.FromSeconds(5),
                    Period = TimeSpan.FromSeconds(1)
                });

            // Act
            var middlewareInstance = new ThrottleServiceMiddleware(next: (innerHttpContext) =>
            {
                innerHttpContext.Response.WriteAsync("ABC");
                return Task.CompletedTask;
            });

            await middlewareInstance.Invoke(defaultContext, throttleServiceMock.Object, throttleConfigurationOptionsMock.Object);

            // Assert
            defaultContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(defaultContext.Response.Body).ReadToEnd();
            Assert.Equal("ABC", body);
            Assert.Equal((int)HttpStatusCode.OK, defaultContext.Response.StatusCode);
            throttleServiceMock.Verify(x => x.IsAllowed(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Middleware_ShouldReturnTooManyRequests_WhenRequestIsNotAllowed()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Request.Path = "/";
            defaultContext.Connection.RemoteIpAddress = faker.Internet.IpAddress();

            throttleServiceMock
                .Setup(x => x.IsAllowed(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            throttleConfigurationOptionsMock
                .SetupGet(x => x.Value)
                .Returns(new ThrottleConfiguration
                {
                    ErrorMessage = "Testing throttling.",
                    MaxRequests = 5,
                    PenaltyTime = TimeSpan.FromSeconds(5),
                    Period = TimeSpan.FromSeconds(1)
                });

            // Act
            var middlewareInstance = new ThrottleServiceMiddleware(next: (innerHttpContext) => Task.CompletedTask);

            await middlewareInstance.Invoke(defaultContext, throttleServiceMock.Object, throttleConfigurationOptionsMock.Object);

            // Assert
            defaultContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(defaultContext.Response.Body).ReadToEnd();
            Assert.Equal(throttleConfigurationOptionsMock.Object.Value.ErrorMessage, body);
            Assert.Equal((int)HttpStatusCode.TooManyRequests, defaultContext.Response.StatusCode);
            throttleServiceMock.Verify(x => x.IsAllowed(It.IsAny<string>()), Times.Once);
        }
    }
}
