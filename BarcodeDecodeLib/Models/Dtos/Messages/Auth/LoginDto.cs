namespace BarcodeDecodeLib.Models.Dtos.Messages.Auth;

public class LoginDto : HttpMessage
{
    public LoginDto()
    {
        CorrelationId = Guid.NewGuid();
    }

    public string Username { get; set; }
    public string Password { get; set; }
}