namespace Shared.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Pooling { get; set; } = true;

    public string ToConnectionString() =>
        $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};Pooling={Pooling}";
}
