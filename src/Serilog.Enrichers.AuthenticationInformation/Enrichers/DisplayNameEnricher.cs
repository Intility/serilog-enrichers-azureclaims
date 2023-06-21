using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Enrichers.AuthenticationInformation;

public class DisplayNameEnricher : ILogEventEnricher
{
    private const string DisplayNamePropertyName = "DisplayName";
    private const string DisplayNameItemKey = "Serilog_DisplayName";

    private readonly IHttpContextAccessor _contextAccessor;

    public DisplayNameEnricher()
    {
        _contextAccessor = new HttpContextAccessor();
    }

    public DisplayNameEnricher(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext is null)
            return;

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return;

        if (httpContext!.Items[DisplayNameItemKey] is LogEventProperty logEventProperty)
        {
            logEvent.AddPropertyIfAbsent(logEventProperty);
            return;
        }

        var displayName = httpContext?.User?.GetDisplayName();
        if (string.IsNullOrEmpty(displayName))
            displayName = "unknown";

        var displayNameProperty = new LogEventProperty(DisplayNamePropertyName, new ScalarValue(displayName));
        httpContext!.Items.Add(DisplayNameItemKey, displayNameProperty);

        logEvent.AddPropertyIfAbsent(displayNameProperty);
    }
}

