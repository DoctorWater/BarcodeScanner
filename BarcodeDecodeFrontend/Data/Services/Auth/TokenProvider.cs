using BarcodeDecodeFrontend.Data.Services.Interfaces;

namespace BarcodeDecodeFrontend.Data.Services.Auth;

public class TokenProvider : ITokenProvider
{
    public string? Token { get; set; }
}