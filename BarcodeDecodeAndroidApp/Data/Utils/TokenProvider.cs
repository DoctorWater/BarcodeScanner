namespace MauiAndroid.App.Utils;
public class TokenProvider : ITokenProvider
{
    private const string TokenKey = "jwt_token";

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.Default.GetAsync(TokenKey);
    }

    public async Task SaveTokenAsync(string token)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
    }

    public void ClearToken()
    {
        SecureStorage.Default.Remove(TokenKey);
    }
}