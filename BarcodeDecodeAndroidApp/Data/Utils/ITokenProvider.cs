namespace MauiAndroid.App.Utils;
public interface ITokenProvider
{
    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token);
    void ClearToken();
}