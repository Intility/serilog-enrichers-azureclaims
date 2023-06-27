using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace Serilog.Enrichers.AzureClaims;

public class OIDEnricher : BaseEnricher
{
    private const string OIDItemKey = "Serilog_OID";
    private const string OIDPropertyName = "ObjectIdentifier";

    public OIDEnricher() : base(OIDItemKey, OIDPropertyName) { }

    public OIDEnricher(IHttpContextAccessor contextAccessor) : base(contextAccessor, OIDItemKey, OIDPropertyName) { }

    protected override string? GetPropertyValue(ClaimsPrincipal user)
    {
        return user?.GetObjectId();
    }
}
