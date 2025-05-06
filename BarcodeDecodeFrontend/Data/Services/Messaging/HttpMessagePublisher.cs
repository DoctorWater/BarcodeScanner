using System.Net;
using System.Net.Http.Headers;
using BarcodeDecodeFrontend.Data.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Configs;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using Microsoft.Extensions.Options;

namespace BarcodeDecodeFrontend.Data.Services.Messaging
{
    public class HttpMessagePublisher : IHttpMessagePublisher
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        private readonly string _baseAddress;

        public HttpMessagePublisher(
            IHttpClientFactory httpClientFactory,
            IOptions<HttpAddresses> addresses, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
            _baseAddress = addresses.Value.BarcodeDecodeBackendAddress;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("API");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenProvider.Token ?? string.Empty);
            client.BaseAddress = new Uri(_baseAddress);
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }

        public async Task<BarcodeResponseMessageBatch> SendBarcodeRequest(
            BarcodeRequestMessageBatch message,
            CancellationToken cancellationToken = default)
        {
            using var client = CreateClient();
            var response = await client.PostAsJsonAsync("api/barcode/batch", message, cancellationToken);
            if(response.StatusCode is HttpStatusCode.Unauthorized) 
                throw new UnauthorizedAccessException("Авторизация для изменения TSU не прошла. Проверьте токен авторизации.");
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                throw new HttpRequestException(
                    $"Ошибка при отправке запроса. Статус: {response.StatusCode}. Причина: {response.ReasonPhrase}");
            }
            return await response.Content.ReadFromJsonAsync<BarcodeResponseMessageBatch>(cancellationToken: cancellationToken)!;
        }

        public async Task<TsuResponseMessage> SendTsuChangeMessage(
            TsuChangeMessage message,
            CancellationToken cancellationToken = default)
        {
            using var client = CreateClient();
            var response = await client.PostAsJsonAsync("api/tsu/change", message, cancellationToken);
            if(response.StatusCode is HttpStatusCode.Unauthorized) 
                throw new UnauthorizedAccessException("Авторизация для изменения TSU не прошла. Проверьте токен авторизации.");
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                throw new HttpRequestException(
                    $"Ошибка при отправке TSU-сообщения. Статус: {response.StatusCode}. Причина: {response.ReasonPhrase}");
            }
            return await response.Content.ReadFromJsonAsync<TsuResponseMessage>(cancellationToken: cancellationToken)!;
        }

        public async Task<TransportOrderResponseMessage> SendTransportOrderChangeMessage(
            TransportOrderChangeMessage message,
            CancellationToken cancellationToken = default)
        {
            using var client = CreateClient();
            var response = await client.PostAsJsonAsync("api/order/change", message, cancellationToken);
            if(response.StatusCode is HttpStatusCode.Unauthorized) 
                throw new UnauthorizedAccessException("Авторизация для изменения TSU не прошла. Проверьте токен авторизации.");
            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                throw new HttpRequestException(
                    $"Ошибка при отправке сообщения об изменении заказа. Статус: {response.StatusCode}. Причина: {response.ReasonPhrase}");
            }
            return await response.Content.ReadFromJsonAsync<TransportOrderResponseMessage>(cancellationToken: cancellationToken)!;
        }

        public async Task<bool> SendTransportOrderRelaunchMessage(
            TransportOrderRelaunchMessage message,
            CancellationToken cancellationToken = default)
        {
            using var client = CreateClient();
            var response = await client.PostAsJsonAsync("api/order/relaunch", message, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        public async Task<LoginResult?> SendLoginMessage(LoginDto message, CancellationToken cancellationToken = default)
        {
            using var client = CreateClient();
            var resp = await client.PostAsJsonAsync("api/auth/login", message);
            if (!resp.IsSuccessStatusCode)
                return null;
            return await resp.Content.ReadFromJsonAsync<LoginResult>(cancellationToken: cancellationToken)!;
        }
    }
}
