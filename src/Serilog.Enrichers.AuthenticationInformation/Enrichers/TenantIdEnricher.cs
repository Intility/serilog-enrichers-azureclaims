﻿using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using Serilog.Enrichers.AuthenticationInformation.Enrichers;
using System.Security.Claims;

namespace Serilog.Enrichers.AuthenticationInformation;

public class TenantIdEnricher : BaseEnricher
{
    private const string TenantIdItemKey = "Serilog_TenantId";
    private const string TenantIdPropertyName = "TenantId";

    public TenantIdEnricher() : base(TenantIdItemKey, TenantIdPropertyName) { }

    public TenantIdEnricher(IHttpContextAccessor contextAccessor) : base(contextAccessor, TenantIdItemKey, TenantIdPropertyName) { }

    protected override string? GetPropertyValue(ClaimsPrincipal user)
    {
        return user?.GetTenantId();
    }
}

