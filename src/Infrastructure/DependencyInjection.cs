using Application.Abstractions;
using Application.Tenants;
using Application.WorkItems;
using Infrastructure.Clock;
using Infrastructure.Options;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SqlServerOptions>()
            .Bind(configuration.GetSection(SqlServerOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.AppDb), "ConnectionStrings:AppDb is required.")
            .ValidateOnStart();

        services.AddOptions<AzureOptions>()
            .Bind(configuration.GetSection(AzureOptions.SectionName));

        string connectionString = configuration.GetConnectionString("AppDb")
            ?? throw new InvalidOperationException("Connection string 'AppDb' is required.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServer => sqlServer.EnableRetryOnFailure());
        });

        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<AppDbContext>());
        services.AddScoped<IWorkItemRepository, WorkItemRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<WorkItemService>();
        services.AddScoped<TenantService>();

        return services;
    }
}
