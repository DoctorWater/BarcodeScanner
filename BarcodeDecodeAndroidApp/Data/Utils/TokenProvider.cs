namespace MauiAndroid.App.Data.Utils;
public class TokenProvider : ITokenProvider
{
    private const string TokenKey = "jwt_token";
    private DateTimeOffset? _tokenExpiration;

    public async Task<string?> GetTokenAsync()
    {
        var token = await SecureStorage.Default.GetAsync(TokenKey);
        if (token != null && _tokenExpiration.HasValue && DateTimeOffset.UtcNow > _tokenExpiration.Value)
        {
            ClearToken();
            return null;
        }

        return token;
    }

    

    public async Task SaveTokenAsync(string token, DateTimeOffset expiration)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
        _tokenExpiration = expiration;
    }

    public void ClearToken()
    {
        SecureStorage.Default.Remove(TokenKey);
    }
}