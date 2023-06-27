using Microsoft.AspNetCore.Http;
using Moq;
using Serilog.Enrichers.AzureClaims.Tests.Helpers;
using Serilog.Events;
using System.Security.Claims;
using Xunit;

namespace Serilog.Enrichers.AzureClaims.Tests
{
    public class AppIdEnricherTests
    {
        [Fact]
        public void LogEvent_DoesNotContainAppIdWhenUserIsNotLoggedIn()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var upnEnricher = new AppIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(upnEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"AppId property is not set when user is not logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.False(evt.Properties.ContainsKey("AppId"));
        }

        [Fact]
        public void LogEvent_ContainsUnknownAppIdWhenUserIsLoggedInButAppIdIsNotFound()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(TestClaimsProvider.NotValidClaims().GetClaimsPrincipal());

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var upnEnricher = new AppIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(upnEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"AppId property is set to unknown when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("AppId"));
            Assert.Equal("unknown", evt.Properties["AppId"].LiteralValue().ToString());
        }

        [Theory]
        [InlineData("azp")]
        [InlineData("appid")]
        public void LogEvent_ContainAppIdWhenUserIsLoggedIn(string claim)
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimValue = Guid.NewGuid().ToString();
            var user = new ClaimsPrincipal(TestClaimsProvider.ValidClaims().AddClaim(claim, claimValue).GetClaimsPrincipal());

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var upnEnricher = new AppIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(upnEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"AppId property is set when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("AppId"));
            Assert.Equal(claimValue, evt.Properties["AppId"].LiteralValue().ToString());
        }
    }
}
