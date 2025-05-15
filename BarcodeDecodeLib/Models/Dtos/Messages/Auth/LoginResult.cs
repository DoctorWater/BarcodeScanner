namespace BarcodeDecodeLib.Models.Dtos.Messages.Auth;

public class LoginResult : HttpMessage
{
    public string Token { get; init; } = null!;
    public DateTimeOffset TokenExpiration { get; init; }
}