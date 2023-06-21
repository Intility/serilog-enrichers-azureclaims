using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System.Security.Claims;

namespace Serilog.Enrichers.AuthenticationInformation;

public class UPNEnricher : ILogEventEnricher
{
    private const string UPNPropertyName = "UserPrincipalName";
    private const string UPNItemKey = "Serilog_UPN";

    private readonly IHttpContextAccessor _contextAccessor;
    public UPNEnricher()
    {
        _contextAccessor = new HttpContextAccessor();
    }

    public UPNEnricher(IHttpContextAccessor contextAccessor)
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

        if (httpContext!.Items[UPNItemKey] is LogEventProperty logEventProperty)
        {
            logEvent.AddPropertyIfAbsent(logEventProperty);
            return;
        }

        var upn = httpContext?.User?.FindFirst(ClaimTypes.Upn)?.Value;
        if (string.IsNullOrEmpty(upn))
            upn = "unknown";

        var upnProperty = new LogEventProperty(UPNPropertyName, new ScalarValue(upn));
        httpContext!.Items.Add(UPNItemKey, upnProperty);

        logEvent.AddPropertyIfAbsent(upnProperty);
    }
}

