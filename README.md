# Serilog.Enrichers.AzureAuthInfo

[![Build_and_Test](https://github.com/Intility/serilog-enricher-azureauthinfo/actions/workflows/Build_and_Test.yml/badge.svg)](https://github.com/Intility/serilog-enricher-azureauthinfo/actions/workflows/Build_and_Test.yml)

Enriches Serilog events with information from the ClaimsPrincipal.

Install the _Serilog.Enrichers.AzureAuthInfo_ [NuGet package](https://www.example.com/)

```powershell
Install-Package Serilog.Enrichers.AzureAuthInfo
```

Then, apply the enricher to your `LoggerConfiguration`:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithUPN()
    .Enrich.WithDisplayName()
    .Enrich.TenantId()
    .Enrich.WithObjectIdentifier()
    // ...other configuration...
    .CreateLogger();
```


### Included enrichers

The package includes:

 * `WithUPN()` - adds `UserPrincipalName` based on the ClaimType `http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn`
 * `WithDisplayName()` - adds `DisplayName` based on the ClaimType `http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name` or `name` or `preferred_username`
 * `WithTenantId()` - adds `TenantId` based on the ClaimType `http://schemas.microsoft.com/identity/claims/tenantid` or `tid` 
 * `WithObjectIdentifier()` - adds `ObjectIdentifier` based on the ClaimType `http://schemas.microsoft.com/identity/claims/objectidentifier` or `oid`

## Installing into an ASP.NET Core Web Application
You need to register the `IHttpContextAccessor` singleton so the enrichers have access to the requests `HttpContext` to extract the data.
This is what your `Program` class should contain in order for this enricher to work as expected:

```cs
// ...
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilog(new LoggerConfiguration()
    .Enrich.WithDisplayName()
    .Enrich.WithUPN()
    .Enrich.WithTenantId()
    .Enrich.WithObjectIdentifier()
    .CreateLogger());

// ...
builder.Services.AddHttpContextAccessor();
// ...

var app = builder.Build();
app.UseSerilogRequestLogging();
// ...

```
