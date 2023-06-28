using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace Serilog.Enrichers.AzureClaims;

/// <summary>
/// Enriches log events with the Object Identifier (OID) property from the user's claims.
/// </summary>
public class OIDEnricher : BaseEnricher
{
    private const string OIDItemKey = "Serilog_OID";
    private const string OIDPropertyName = "ObjectIdentifier";

    /// <summary>
    /// Initializes a new instance of the <see cref="OIDEnricher"/> class.
    /// </summary>
    public OIDEnricher() : base(OIDItemKey, OIDPropertyName) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OIDEnricher"/> class with the specified HTTP context accessor.
    /// </summary>
    /// <param name="contextAccessor">The HTTP context accessor to use for retrieving the user's claims.</param>
    public OIDEnricher(IHttpContextAccessor contextAccessor) : base(contextAccessor, OIDItemKey, OIDPropertyName) { }

    /// <summary>
    /// Gets the Object Identifier (OID) property value from the specified claims principal.
    /// </summary>
    /// <param name="user">The claims principal representing the user.</param>
    /// <returns>The Object Identifier (OID) property value, or <c>null</c> if it cannot be found.</returns>
    protected override string? GetPropertyValue(ClaimsPrincipal user)
    {
        return user?.GetObjectId();
    }
}

