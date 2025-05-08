using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Models;

namespace MauiAndroid.App.Pages
{
    public partial class BarcodeVerificationPage : ContentPage
    {
        private readonly BarcodeService _barcodeService = new();

        public string BarcodeValue { get; set; }

        public BarcodeVerificationPage(string barcode)
        {
            InitializeComponent();
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
            var batch   = new BarcodeRequestMessageBatch(new List<BarcodeRequestMessage> { message });

            var responseData = await _barcodeService.SendBarcodeRequest(batch);

            if (responseData is null)
            {
                await DisplayAlert("Ошибка", "Не удалось получить данные от сервера.", "OK");
                return;
            }

            var mappedData = new BackendResponseViewModel
            {
                TransportOrders      = responseData.Messages
                                               .SelectMany(m => m.TransportOrders)
                                               .Select(to => new TransportOrderViewModel(to))
                                               .ToList(),
                TransportStorageUnits = responseData.Messages
                                               .SelectMany(m => m.TransportStorageUnits)
                                               .Select(tu => new TransportStorageUnitViewModel(tu))
                                               .ToList()
            };

            await Navigation.PushAsync(
                new ClientDataObservePage(
                    responseData.Messages
                                .SelectMany(dto => dto.TransportStorageUnits)
                                .Select(unit => new TransportStorageUnitViewModel(unit)))
            );
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
