using Microsoft.AspNetCore.Http;
using Moq;
using Serilog.Enrichers.AzureClaims.Tests.Helpers;
using Serilog.Events;
using System.Security.Claims;
using Xunit;

namespace Serilog.Enrichers.AzureClaims.Tests
{
    public class TenantIdEnricherTests
    {
        [Fact]
        public void LogEvent_DoesNotContainTenantIdWhenUserIsNotLoggedIn()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var tenantIdEnricher = new TenantIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(tenantIdEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"TenantId property is not set when user is not logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.False(evt.Properties.ContainsKey("TenantId"));
        }

        [Fact]
        public void LogEvent_ContainsUnknownTenantIdWhenUserIsLoggedInButTenantIdIsNotFound()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(TestClaimsProvider.NotValidClaims().GetClaimsPrincipal());

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var tenantIdEnricher = new TenantIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(tenantIdEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"TenantId property is set to unknown when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("TenantId"));
            Assert.Equal("unknown", evt.Properties["TenantId"].LiteralValue().ToString());
        }

        [Fact]
        public void LogEvent_ContainTenantIdWhenUserIsLoggedIn()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(TestClaimsProvider.ValidClaims().GetClaimsPrincipal());

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = user
            });

            var tenantIdEnricher = new TenantIdEnricher(httpContextAccessorMock.Object);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(tenantIdEnricher)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            // Act
            log.Information(@"TenantId property is set when user is logged in");

            // Assert
            Assert.NotNull(evt);
            Assert.True(evt.Properties.ContainsKey("TenantId"));
            Assert.Equal(TestConstants.TENANTID, evt.Properties["TenantId"].LiteralValue().ToString());
        }
    }
}
