namespace Infrastructure.Options;

public sealed class SqlServerOptions
{
    public const string SectionName = "ConnectionStrings";

    public string AppDb { get; init; } = string.Empty;
}
