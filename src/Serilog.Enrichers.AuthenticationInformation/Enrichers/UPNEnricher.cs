using Microsoft.AspNetCore.Http;
using Serilog.Enrichers.AuthenticationInformation.Enrichers;
using System.Security.Claims;

namespace Serilog.Enrichers.AuthenticationInformation;

public class UPNEnricher : BaseEnricher
{
    private const string UPNItemKey = "Serilog_UPN";
    private const string UPNPropertyName = "UserPrincipalName";

    public UPNEnricher() : base(UPNItemKey, UPNPropertyName) { }

    public UPNEnricher(IHttpContextAccessor contextAccessor) : base(contextAccessor, UPNItemKey, UPNPropertyName) { }

    protected override string GetPropertyValue(ClaimsPrincipal user)
    {
        return user?.FindFirst(ClaimTypes.Upn)?.Value ?? UnknownValue;
    }
}

