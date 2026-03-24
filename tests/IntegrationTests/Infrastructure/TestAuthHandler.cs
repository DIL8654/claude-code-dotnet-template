using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace IntegrationTests.Infrastructure;

public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "TestScheme";
    public const string ClaimsHeader = "X-Test-Claims";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ClaimsHeader, out StringValues values)
            || string.IsNullOrEmpty(values))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        ClaimDto[]? claimDtos = JsonSerializer.Deserialize<ClaimDto[]>(
            values!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (claimDtos is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        List<Claim> claims = claimDtos
            .Select(c => new Claim(c.Type, c.Value))
            .ToList();

        claims.Add(new Claim(ClaimTypes.Name, "test-user"));

        ClaimsIdentity identity = new(claims, SchemeName);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private sealed record ClaimDto(string Type, string Value);
}
