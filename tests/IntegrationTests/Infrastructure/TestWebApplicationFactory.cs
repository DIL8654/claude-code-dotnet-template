using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.Infrastructure;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        builder.UseEnvironment("IntegrationTesting");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));

            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    public void EnsureDatabaseCreated()
    {
        using HttpClient _ = CreateClient();
        using IServiceScope scope = Services.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }

    public HttpClient CreateAuthenticatedClient(params (string Type, string Value)[] claims)
    {
        HttpClient client = CreateClient();
        string claimsJson = System.Text.Json.JsonSerializer.Serialize(
            claims.Select(c => new { c.Type, c.Value }));
        client.DefaultRequestHeaders.Add(TestAuthHandler.ClaimsHeader, claimsJson);
        return client;
    }
}
