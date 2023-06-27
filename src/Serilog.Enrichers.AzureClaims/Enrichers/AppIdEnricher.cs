using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Serilog.Enrichers.AzureClaims;

public class AppIdEnricher : BaseEnricher
{
    private const string AzpItemKey = "Serilog_AppId";
    private const string AzpPropertyName = "AppId";

    public AppIdEnricher() : base(AzpItemKey, AzpPropertyName) { }

    public AppIdEnricher(IHttpContextAccessor contextAccessor) : base(contextAccessor, AzpItemKey, AzpPropertyName) { }

    protected override string? GetPropertyValue(ClaimsPrincipal user)
    {
        return user?.FindFirst("appid")?.Value ?? user?.FindFirst("azp")?.Value;
    }
}
