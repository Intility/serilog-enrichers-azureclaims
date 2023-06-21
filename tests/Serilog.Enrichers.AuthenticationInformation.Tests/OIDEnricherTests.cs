using Microsoft.AspNetCore.Http;
using Moq;
using Serilog.Enrichers.AuthenticationInformation.Tests.Helpers;
using Serilog.Events;
using System.Security.Claims;
using Xunit;

namespace Serilog.Enrichers.AuthenticationInformation.Tests
{
    public class OIDEnricherTests
    {
        [Fact]
        public void LogEvent_DoesNotContainOIDWhenUserIsNotLoggedIn()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var oidEnricher = new OIDEnricher(httpContextAccessorMock.Object);

            LogEvent? evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(oidEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"ObjectIdentifier property is not set when user is not logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.False(evt.Properties.ContainsKey("ObjectIdentifier"));
        }

        [Fact]
        public void LogEvent_ContainsUnknownOIDWhenUserIsLoggedInButOIDIsNotFound()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(TestClaimsProvider.NotValidClaims().GetClaimsPrincipal());

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var oidEnricher = new OIDEnricher(httpContextAccessorMock.Object);

            LogEvent? evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(oidEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"ObjectIdentifier property is set to unknown when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("ObjectIdentifier"));
            Assert.Equal("unknown", evt.Properties["ObjectIdentifier"].LiteralValue().ToString());
        }

        [Fact]
        public void LogEvent_ContainOIDWhenUserIsLoggedIn()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(TestClaimsProvider.ValidClaims().GetClaimsPrincipal());

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var oidEnricher = new OIDEnricher(httpContextAccessorMock.Object);

            LogEvent? evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(oidEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"ObjectIdentifier property is set when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("ObjectIdentifier"));
            Assert.Equal(TestConstants.OID, evt.Properties["ObjectIdentifier"].LiteralValue().ToString());
        }
    }
}
