using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Serilog.Enrichers.AzureAuthInfo;

public class AppIdEnricher : BaseEnricher
{
    private const string AztItemKey = "Serilog_AppId";
    private const string AztPropertyName = "AppId";

    public AppIdEnricher() : base(AztItemKey, AztPropertyName) { }

    public AppIdEnricher(IHttpContextAccessor contextAccessor) : base(contextAccessor, AztItemKey, AztPropertyName) { }

    protected override string? GetPropertyValue(ClaimsPrincipal user)
    {
        return user?.FindFirst("appid")?.Value ?? user?.FindFirst("azp")?.Value;
    }
}
