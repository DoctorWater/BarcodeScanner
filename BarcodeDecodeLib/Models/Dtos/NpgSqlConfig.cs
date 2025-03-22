namespace BarcodeDecodeLib.Models.Dtos;

public record NpgSqlConfig
{
    public string Database { get; set; } = string.Empty;
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int CommandTimeoutSeconds { get; set; } = 30;
    public int TimeoutSeconds { get; set; }
    public int KeepAliveSeconds { get; set; }
    public bool IncludeErrorDetail { get; set; } = false;
    public bool EnableDetailedErrors { get; set; } = false;
    public bool Pooling { get; set; } = false;
    public bool Enlist { get; set; } = false;
    public int MaxPoolSize { get; set; } = 100;
}