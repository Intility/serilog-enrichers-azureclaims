using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Enrichers.AuthenticationInformation;

public class TenantIdEnricher : ILogEventEnricher
{
    private const string TenantIdPropertyName = "TenantId";
    private const string TenantIdItemKey = "Serilog_TenantId";

    private readonly IHttpContextAccessor _contextAccessor;

    public TenantIdEnricher()
    {
        _contextAccessor = new HttpContextAccessor();
    }

    public TenantIdEnricher(IHttpContextAccessor contextAccessor)
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

        if (httpContext!.Items[TenantIdItemKey] is LogEventProperty logEventProperty)
        {
            logEvent.AddPropertyIfAbsent(logEventProperty);
            return;
        }

        var tenantId = httpContext?.User?.GetTenantId();
        if (string.IsNullOrEmpty(tenantId))
            tenantId = "unknown";

        var tenantIdProperty = new LogEventProperty(TenantIdPropertyName, new ScalarValue(tenantId));
        httpContext!.Items.Add(TenantIdItemKey, tenantIdProperty);

        logEvent.AddPropertyIfAbsent(tenantIdProperty);
    }
}

