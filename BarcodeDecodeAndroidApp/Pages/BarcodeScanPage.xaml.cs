using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarcodeScanner.Mobile;
using MauiAndroid.App.Data.Services;

namespace MauiAndroid.App.Pages;

public partial class BarcodeScanPage : ContentPage
{
    private IHttpMessagingService _barcodeService = App.Services.GetRequiredService<IHttpMessagingService>();
    public BarcodeScanPage()
    {
        InitializeComponent();

        Init().Wait();
    }

    private async Task Init()
    {
        var status = await CheckAndRequestCameraPermission();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Разрешение необходимо", "Приложению необходимо разрешение на использование камеры.",
                "OK");
            return;
        }

        var camView = new CameraView
        {
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true),
            TorchOn = false,
            VibrationOnDetected = false,
            ScanInterval = 300
        };
        camView.OnDetected += Camera_OnDetected;
        CamViewFrame.Content = camView;
    }
    
    private async void Camera_OnDetected(object sender, OnDetectedEventArg e)
    {
        var obj = e.BarcodeResults.First();
        
        await Navigation.PushAsync(new BarcodeVerificationPage(obj.DisplayValue));
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
}