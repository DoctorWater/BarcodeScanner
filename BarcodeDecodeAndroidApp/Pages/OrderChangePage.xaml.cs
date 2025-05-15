using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using BarcodeDecodeLib.Models.Enums;
using MauiAndroid.App.Data.Services;

namespace MauiAndroid.App.Pages;
public partial class OrderChangePage : ContentPage
{
    private readonly int _orderId;
    private readonly IHttpMessagingService _messagingService = App.Services.GetRequiredService<IHttpMessagingService>();

    public OrderChangePage(TransportOrderViewModel vm)
    {
        InitializeComponent();

        _orderId = vm.Id;

        IdLabel.Text = vm.Id.ToString();
        BarcodeEntry.Text = vm.Barcode;
        
        foreach (TransportOrderStatusEnum s in Enum.GetValues(typeof(TransportOrderStatusEnum)))
            StatusPicker.Items.Add(s.ToString());
        
        if (vm.Status != null)
        {
            var name = vm.Status.ToString();
            var idx = StatusPicker.Items.IndexOf(name);
            if (idx >= 0)
                StatusPicker.SelectedIndex = idx;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var barcode = BarcodeEntry.Text;
        
        TransportOrderStatusEnum? status = null;
        if (StatusPicker.SelectedIndex >= 0)
        {
            var sel = StatusPicker.Items[StatusPicker.SelectedIndex];
            status = (TransportOrderStatusEnum)Enum.Parse(
                typeof(TransportOrderStatusEnum), sel);
        }
        
        var msg = new TransportOrderChangeMessage(_orderId)
        {
            Barcode = barcode,
            Status = status
        };

        try
        {
            await _messagingService.SendTransportOrderChangeMessage(msg);
        }
        catch (Exception exception)
        {
            DisplayAlert("Ошибка", exception.Message, "OK");
        }
        
        await Navigation.PopAsync();
    }
}