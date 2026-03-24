using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Infrastructure;
using WebApi.Contracts;

namespace IntegrationTests.WebApi;

public sealed class TenantsEndpointsTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>
{
    [Fact]
    public async Task CreateAsync_ShouldReturnCreated_WhenCallerHasBothPermissionAndRole()
    {
        factory.EnsureDatabaseCreated();
        HttpClient client = factory.CreateAuthenticatedClient(
            ("permissions", "create.tenant"),
            ("roles", "TenantManager"));

        CreateTenantHttpRequest request = new()
        {
            TenantName = "Acme Corp",
            Description = "Integration test tenant",
            Slug = "acme-corp"
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/tenants", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnForbidden_WhenPermissionClaimIsMissing()
    {
        factory.EnsureDatabaseCreated();
        HttpClient client = factory.CreateAuthenticatedClient(
            ("roles", "TenantManager"));

        CreateTenantHttpRequest request = new()
        {
            TenantName = "No Permission Corp",
            Slug = "no-permission-corp"
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/tenants", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnForbidden_WhenRoleClaimIsMissing()
    {
        factory.EnsureDatabaseCreated();
        HttpClient client = factory.CreateAuthenticatedClient(
            ("permissions", "create.tenant"));

        CreateTenantHttpRequest request = new()
        {
            TenantName = "No Role Corp",
            Slug = "no-role-corp"
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/tenants", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnUnauthorized_WhenCallerIsNotAuthenticated()
    {
        factory.EnsureDatabaseCreated();
        HttpClient client = factory.CreateClient();

        CreateTenantHttpRequest request = new()
        {
            TenantName = "Anon Corp",
            Slug = "anon-corp"
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/tenants", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnBadRequest_WhenPayloadIsInvalid()
    {
        factory.EnsureDatabaseCreated();
        HttpClient client = factory.CreateAuthenticatedClient(
            ("permissions", "create.tenant"),
            ("roles", "TenantManager"));

        var invalidRequest = new { Description = "No name or slug provided" };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/tenants", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnBadRequest_WhenSlugFormatIsInvalid()
    {
        factory.EnsureDatabaseCreated();
        HttpClient client = factory.CreateAuthenticatedClient(
            ("permissions", "create.tenant"),
            ("roles", "TenantManager"));

        CreateTenantHttpRequest request = new()
        {
            TenantName = "Bad Slug Corp",
            Slug = "-invalid-slug-"
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/tenants", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
