using Serilog.Configuration;
using Serilog.Enrichers.AzureAuthInfo;

namespace Serilog;

/// <summary>
///     Extension methods for setting up azure auth enrichers <see cref="LoggerEnrichmentConfiguration"/>.
/// </summary>
public static class AuthenticationInformationLoggerConfigurationExtensions
{
    /// <summary>
    /// Adds a display name enrichment to the logger configuration.
    /// </summary>
    /// <param name="enrichmentConfiguration">The logger enrichment configuration.</param>
    /// <returns>The logger configuration with the display name enrichment added.</returns>
    public static LoggerConfiguration WithDisplayName(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<DisplayNameEnricher>();
    }

    /// <summary>
    /// Adds a user principal name (UPN) enrichment to the logger configuration.
    /// </summary>
    /// <param name="enrichmentConfiguration">The logger enrichment configuration.</param>
    /// <remarks>
    /// The user principal name (UPN) is an optional claim in the v2 tokens, but will always be present in v1 tokens.
    /// </remarks>
    /// <returns>The logger configuration with the UPN enrichment added.</returns>
    public static LoggerConfiguration WithUPN(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<UPNEnricher>();
    }

    /// <summary>
    /// Adds an object identifier (OID) enrichment to the logger configuration.
    /// </summary>
    /// <param name="enrichmentConfiguration">The logger enrichment configuration.</param>
    /// <returns>The logger configuration with the OID enrichment added.</returns>
    public static LoggerConfiguration WithOID(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<OIDEnricher>();
    }

    /// <summary>
    /// Adds a tenant ID (TID) enrichment to the logger configuration.
    /// </summary>
    /// <param name="enrichmentConfiguration">The logger enrichment configuration.</param>
    /// <returns>The logger configuration with the TID enrichment added.</returns>
    public static LoggerConfiguration WithTID(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<TenantIdEnricher>();
    }

    /// <summary>
    /// Adds a application ID (AppId/AZP) enrichment to the logger configuration.
    /// </summary>
    /// <param name="enrichmentConfiguration">The logger enrichment configuration.</param>
    /// <remarks>
    /// The application ID (AppId/AZP) is named appid in v1 tokens and azp in v2 tokens..
    /// </remarks>
    /// <returns>The logger configuration with the TID enrichment added.</returns>
    public static LoggerConfiguration WithAppId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<TenantIdEnricher>();
    }
}
