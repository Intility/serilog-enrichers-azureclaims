using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System.Security.Claims;

namespace Serilog.Enrichers.AuthenticationInformation.Enrichers
{
    public abstract class BaseEnricher : ILogEventEnricher
    {
        protected const string UnknownValue = "unknown";
        protected IHttpContextAccessor _contextAccessor;
        protected string ItemKey;
        protected string PropertyName;

        protected BaseEnricher(string itemKey, string propertyName)
        {
            _contextAccessor = new HttpContextAccessor();
            ItemKey = itemKey;
            PropertyName = propertyName;
        }

        protected BaseEnricher(IHttpContextAccessor contextAccessor, string itemKey, string propertyName)
        {
            _contextAccessor = contextAccessor;
            ItemKey = itemKey;
            PropertyName = propertyName;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext is null)
                return;

            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return;

            if (httpContext!.Items[ItemKey] is LogEventProperty logEventProperty)
            {
                logEvent.AddPropertyIfAbsent(logEventProperty);
                return;
            }

            var propertyValue = GetPropertyValue(httpContext?.User!);
            if (string.IsNullOrEmpty(propertyValue))
                propertyValue = UnknownValue;

            var evtProperty = new LogEventProperty(PropertyName, new ScalarValue(propertyValue));
            httpContext!.Items.Add(ItemKey, evtProperty);

            logEvent.AddPropertyIfAbsent(evtProperty);
        }

        protected abstract string GetPropertyValue(ClaimsPrincipal user);
    }
}
