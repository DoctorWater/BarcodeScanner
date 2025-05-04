using System.Net.Http.Headers;
using BarcodeDecodeFrontend.Data.Services.Interfaces;


namespace BarcodeDecodeFrontend.Data.Services.Auth;

public class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public JwtAuthorizationMessageHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenProvider.Token;
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}