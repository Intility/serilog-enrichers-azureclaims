﻿using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using NSubstitute;
using Serilog.Core;
using Serilog.Enrichers.AzureClaims.Tests.Helpers;
using Serilog.Events;
using System.Security.Claims;
using Xunit;

namespace Serilog.Enrichers.AzureClaims.Tests
{
    public class BaseEnricherTests
    {
        private const string _testKey = "TestKey";
        private const string _testProperty = "TestProperty";
        private const string _testValue = "TestValue";

        [Fact]
        public void Enrich_WhenHttpContextIsNull_ShouldNotAddLogEventProperty()
        {
            // Arrange
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            httpContextAccessorMock.HttpContext.Returns((HttpContext)null);

            var enricher = new TestEnricher(httpContextAccessorMock, _testKey, _testProperty);

            var logEvent = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Information,
                null,
                MessageTemplate.Empty,
                Enumerable.Empty<LogEventProperty>());

            // Act
            enricher.Enrich(logEvent, Substitute.For<ILogEventPropertyFactory>());

            // Assert
            Assert.False(logEvent.Properties.ContainsKey(_testProperty));
        }

        [Fact]
        public void Enrich_WhenUserIsNotAuthenticated_ShouldNotAddLogEventProperty()
        {
            // Arrange
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            httpContextAccessorMock.HttpContext.Returns(new DefaultHttpContext());

            var enricher = new TestEnricher(httpContextAccessorMock, _testKey, _testProperty);

            var logEvent = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Information,
                null,
                MessageTemplate.Empty,
                Enumerable.Empty<LogEventProperty>());

            // Act
            enricher.Enrich(logEvent, Substitute.For<ILogEventPropertyFactory>());

            // Assert
            Assert.False(logEvent.Properties.ContainsKey(_testProperty));
        }

        [Fact]
        public void Enrich_WhenHttpContextItemContainsLogEventProperty_ShouldAddPropertyToLogEvent()
        {
            // Arrange
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            var user = new ClaimsPrincipal(TestClaimsProvider.ValidClaims().GetClaimsPrincipal());
            var logEventProperty = new LogEventProperty(_testProperty, new ScalarValue(_testValue));

            httpContextAccessorMock.HttpContext.Returns(new DefaultHttpContext
            {
                User = user,
                Items = { { _testKey, logEventProperty } }
            });

            var enricher = new TestEnricher(httpContextAccessorMock, _testKey, _testProperty);

            var logEvent = new LogEvent(
                DateTimeOffset.UtcNow,
                LogEventLevel.Information,
                null,
                MessageTemplate.Empty,
                Enumerable.Empty<LogEventProperty>());

            // Act
            enricher.Enrich(logEvent, Substitute.For<ILogEventPropertyFactory>());

            // Assert
            Assert.True(logEvent.Properties.ContainsKey(_testProperty));
            Assert.Equal(logEvent.Properties[_testProperty], logEventProperty.Value);
        }
    }

    internal class TestEnricher : BaseEnricher
    {
        public TestEnricher(string itemKey, string propertyName) : base(itemKey, propertyName)
        {
        }

        public TestEnricher(IHttpContextAccessor contextAccessor, string itemKey, string propertyName) : base(contextAccessor, itemKey, propertyName)
        {
        }

        protected override string GetPropertyValue(ClaimsPrincipal user)
        {
            return user?.GetObjectId();
        }
    }
}
