namespace MauiAndroid.App.Data.Utils;
public interface ITokenProvider
{
    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token, DateTimeOffset expiration);
    void ClearToken();
}