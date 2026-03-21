namespace Infrastructure.Options;

public sealed class AzureOptions
{
    public const string SectionName = "Azure";

    public KeyVaultOptions KeyVault { get; init; } = new();

    public AppConfigurationOptions AppConfiguration { get; init; } = new();

    public StorageOptions Storage { get; init; } = new();

    public ServiceBusOptions ServiceBus { get; init; } = new();
}

public sealed class KeyVaultOptions
{
    public string? VaultUri { get; init; }
}

public sealed class AppConfigurationOptions
{
    public string? Endpoint { get; init; }
}

public sealed class StorageOptions
{
    public string? BlobServiceUri { get; init; }
}

public sealed class ServiceBusOptions
{
    public string? Namespace { get; init; }
}
