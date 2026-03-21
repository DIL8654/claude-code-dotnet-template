using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Infrastructure;
using WebApi.Contracts;

namespace IntegrationTests.WebApi;

public sealed class WorkItemsEndpointsTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>
{
    [Fact]
    public async Task CreateAsync_ShouldReturnCreated_WhenPayloadIsValid()
    {
        factory.EnsureDatabaseCreated();
        HttpClient client = factory.CreateClient();
        CreateWorkItemHttpRequest request = new("Seed template", "Exercise starter path");

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/work-items", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNotFound_WhenResourceDoesNotExist()
    {
        factory.EnsureDatabaseCreated();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync($"/api/v1/work-items/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
