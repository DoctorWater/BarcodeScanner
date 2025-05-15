using System.Net;
using System.Net.Http.Headers;
using MauiAndroid.App.Data.Utils;

namespace MauiAndroid.App.Handlers;
public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public JwtAuthorizationHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}