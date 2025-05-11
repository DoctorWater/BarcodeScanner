namespace BarcodeDecodeFrontend.Data.Services.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    void LogoutAsync();
}