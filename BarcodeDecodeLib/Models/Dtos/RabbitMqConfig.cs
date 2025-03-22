namespace BarcodeDecodeLib.Models.Dtos;

public record RabbitMqConfig
{
    public string HostName { get; set; } = "localhost";
    public ushort Port { get; set; } = 5672;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public bool WaitUntilStarted { get; set; } = true;
    public string EndpointNameFormatterPrefix { get; set; } = "hlc.";
}