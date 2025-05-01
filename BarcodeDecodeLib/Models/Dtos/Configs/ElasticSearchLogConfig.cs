namespace BarcodeDecodeLib.Models.Dtos.Configs;

public record ElasticSearchLogConfig
{
    public Uri Uri { get; set; } = new Uri("http://localhost");
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string IndexFormat { get; set; } = string.Empty;
    public bool AutoRegisterTemplate { get; set; } = true;
}