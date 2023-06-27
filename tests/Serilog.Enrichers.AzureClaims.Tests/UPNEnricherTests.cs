using Microsoft.AspNetCore.Http;
using Moq;
using Serilog.Enrichers.AzureClaims.Tests.Helpers;
using Serilog.Events;
using System.Security.Claims;
using Xunit;

namespace Serilog.Enrichers.AzureClaims.Tests
{
    public class UPNEnricherTests
    {
        [Fact]
        public void LogEvent_DoesNotContainUPNWhenUserIsNotLoggedIn()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var upnEnricher = new UPNEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(upnEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"UPN property is not set when user is not logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.False(evt.Properties.ContainsKey("UserPrincipalName"));
        }

        [Fact]
        public void LogEvent_ContainsUnknownUPNWhenUserIsLoggedInButUPNIsNotFound()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(TestClaimsProvider.NotValidClaims().GetClaimsPrincipal());

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var upnEnricher = new UPNEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(upnEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"UserPrincipalName property is set to unknown when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("UserPrincipalName"));
            Assert.Equal("unknown", evt.Properties["UserPrincipalName"].LiteralValue().ToString());
        }

        [Fact]
        public void LogEvent_ContainUPNWhenUserIsLoggedIn()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(TestClaimsProvider.ValidClaims().GetClaimsPrincipal());

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var upnEnricher = new UPNEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(upnEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"UserPrincipalName property is set when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("UserPrincipalName"));
            Assert.Equal(TestConstants.UPN, evt.Properties["UserPrincipalName"].LiteralValue().ToString());
        }
    }
}
