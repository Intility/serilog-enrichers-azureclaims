using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Enrichers.AuthenticationInformation;

public class OIDEnricher : ILogEventEnricher
{
    private const string OIDPropertyName = "ObjectIdentifier";
    private const string OIDItemKey = "Serilog_OID";

    private readonly IHttpContextAccessor _contextAccessor;

    public OIDEnricher()
    {
        _contextAccessor = new HttpContextAccessor();
    }

    public OIDEnricher(IHttpContextAccessor contextAccessor)
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

        if (httpContext!.Items[OIDItemKey] is LogEventProperty logEventProperty)
        {
            logEvent.AddPropertyIfAbsent(logEventProperty);
            return;
        }

        var oid = httpContext?.User?.GetObjectId();
        if (string.IsNullOrEmpty(oid))
            oid = "unknown";

        var oidProperty = new LogEventProperty(OIDPropertyName, new ScalarValue(oid));
        httpContext!.Items.Add(OIDItemKey, oidProperty);

        logEvent.AddPropertyIfAbsent(oidProperty);
    }
}
