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
        await Navigation.PushAsync(new BarcodeVerificationPage("SomeBarcode1"));
        /* var status = await CheckAndRequestCameraPermission();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Разрешение необходимо", "Приложению необходимо разрешение на использование камеры.", "OK");
            return;
        }
        _isCamViewVisible = !_isCamViewVisible;

        if (_isCamViewVisible)
        {
            var camView = new CameraView
            {
                HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true),
                TorchOn = false,
                VibrationOnDetected = false,
                ScanInterval = 300
            };
            camView.OnDetected += Camera_OnDetected;
            CamViewFrame.Content = camView;
            ScanBtn.Text = "Cancel";
        }
        else
        {
            CamViewFrame.Content = null;
            ScanBtn.Text = "Scan barcode";
        } */
    }

    private async Task<PermissionStatus> CheckAndRequestCameraPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (status == PermissionStatus.Granted)
            return status;

        if (Permissions.ShouldShowRationale<Permissions.Camera>())
        {
            await DisplayAlert("Разрешение необходимо", "Приложению необходимо разрешение на использование камеры для сканирования штрихкодов.", "OK");
        }

        status = await Permissions.RequestAsync<Permissions.Camera>();

        return status;
    }

    private async void Camera_OnDetected(object sender, OnDetectedEventArg e)
    {
        var obj = e.BarcodeResults.First();
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