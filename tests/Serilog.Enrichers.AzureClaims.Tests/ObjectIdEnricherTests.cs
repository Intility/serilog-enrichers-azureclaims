using Microsoft.AspNetCore.Http;
using Moq;
using Serilog.Enrichers.AzureClaims.Tests.Helpers;
using Serilog.Events;
using System.Security.Claims;
using Xunit;

namespace Serilog.Enrichers.AzureClaims.Tests
{
    public class OIDEnricherTests
    {
        [Fact]
        public void LogEvent_DoesNotContainOIDWhenUserIsNotLoggedIn()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var oidEnricher = new ObjectIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(oidEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"ObjectId property is not set when user is not logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.False(evt.Properties.ContainsKey("ObjectId"));
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

            var oidEnricher = new ObjectIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(oidEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"ObjectId property is set to unknown when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("ObjectId"));
            Assert.Equal("unknown", evt.Properties["ObjectId"].LiteralValue().ToString());
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

            var oidEnricher = new ObjectIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(oidEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"ObjectId property is set when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("ObjectId"));
            Assert.Equal(TestConstants.OID, evt.Properties["ObjectId"].LiteralValue().ToString());
        }
    }
}
