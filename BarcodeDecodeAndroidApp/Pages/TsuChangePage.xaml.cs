using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using BarcodeDecodeLib.Models.Enums;
using MauiAndroid.App.Data.Services;

namespace MauiAndroid.App.Pages;
public partial class TsuChangePage : ContentPage
{
    private readonly int _tsuId;
    private readonly IHttpMessagingService _messagingService = App.Services.GetRequiredService<IHttpMessagingService>();

    public TsuChangePage(TransportStorageUnitViewModel vm)
    {
        InitializeComponent();
        
        _tsuId = vm.Id;
        
        IdLabel.Text = vm.Id.ToString();
        BarcodeEntry.Text = vm.Barcode;
        
        foreach (TsuStatusEnum status in Enum.GetValues(typeof(TsuStatusEnum)))
        {
            StatusPicker.Items.Add(status.ToString());
        }

        if (vm.Status != null)
        {
            var current = vm.Status.ToString();
            var idx = StatusPicker.Items.IndexOf(current);
            if (idx >= 0)
                StatusPicker.SelectedIndex = idx;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var barcode = BarcodeEntry.Text;
        
        TsuStatusEnum? status = null;
        if (StatusPicker.SelectedIndex >= 0)
        {
            var selected = StatusPicker.Items[StatusPicker.SelectedIndex];
            status = (TsuStatusEnum)Enum.Parse(typeof(TsuStatusEnum), selected);
        }
        
        var message = new TsuChangeMessage(_tsuId)
        {
            Barcode = barcode,
            Status = status
        };

        try
        {
            await _messagingService.SendTsuChangeMessage(message);
        }
        catch (Exception exception)
        {
            await DisplayAlert("Ошибка", exception.Message, "OK");
        }

        await Navigation.PopAsync();
    }
}