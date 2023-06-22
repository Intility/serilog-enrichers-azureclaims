using Serilog.Configuration;

namespace Serilog;

public static class AuthenticationInformationLoggerConfigurationExtensions
{
    public static LoggerConfiguration WithDisplayName(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<DisplayNameEnricher>();
    }

    public static LoggerConfiguration WithUPN(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<UPNEnricher>();
    }

    public static LoggerConfiguration WithObjectIdentifier(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<OIDEnricher>();
    }

    public static LoggerConfiguration WithTenantId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        if (enrichmentConfiguration is null)
            throw new ArgumentNullException(nameof(enrichmentConfiguration));

        return enrichmentConfiguration.With<TenantIdEnricher>();
    }
}
