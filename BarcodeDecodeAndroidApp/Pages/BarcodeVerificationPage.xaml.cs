using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Models;
using MauiAndroid.App.Data.Services;

namespace MauiAndroid.App.Pages
{
    public partial class BarcodeVerificationPage : ContentPage
    {
        private readonly IHttpMessagingService _messagingService;

        public string BarcodeValue { get; set; }

        public BarcodeVerificationPage(string barcode)
        {
            InitializeComponent();
            _messagingService = App.Services.GetRequiredService<IHttpMessagingService>();
            BarcodeValue = barcode;
            BindingContext = this;
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            var cleanCode = (BarcodeEntry.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(cleanCode))
            {
                await DisplayAlert("Ошибка", "Поле штрих‑кода не может быть пустым.", "OK");
                return;
            }


            var message = new BarcodeRequestMessage(cleanCode);
            var batch = new BarcodeRequestMessageBatch(new List<BarcodeRequestMessage> { message });

            try
            {
                var responseData = await _messagingService.SendBarcodeRequest(batch);
                if (responseData is null)
                {
                    await DisplayAlert("Ошибка", "Не удалось получить данные от сервера.", "OK");
                    return;
                }

                var tsus = responseData.Messages
                    .SelectMany(dto => dto.TransportStorageUnits)
                    .Select(unit => new TransportStorageUnitViewModel(unit));

                await Navigation.PushAsync(
                    new ClientDataObservePage(tsus)
                );
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}