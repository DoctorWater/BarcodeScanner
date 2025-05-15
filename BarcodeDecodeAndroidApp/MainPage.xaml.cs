using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Models;
using BarcodeScanner.Mobile;
using MauiAndroid.App.Pages;

namespace MauiAndroid.App;

public partial class MainPage : ContentPage
{
    private bool _isCamViewVisible = false;
     private readonly LoginPage _loginPage;
    public MainPage(LoginPage loginPage)
    {
        InitializeComponent();
        _loginPage = loginPage;
    }

    [Obsolete]
    private async void OnScanClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new BarcodeScanPage());
    }



    private async void OnSettingsButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }

    private async void OnAuthorizationButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(_loginPage);
    }

    private async void TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo is not null)
            {
                using var stream = await photo.OpenReadAsync();
            }
        }
    }
}