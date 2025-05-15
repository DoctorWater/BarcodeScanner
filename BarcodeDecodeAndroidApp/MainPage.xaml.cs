using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Models;
using BarcodeScanner.Mobile;
using MauiAndroid.App.Data.Utils;
using MauiAndroid.App.Pages;

namespace MauiAndroid.App;

public partial class MainPage : ContentPage
{
    private readonly LoginPage _loginPage;
    private readonly ITokenProvider _tokenProvider;
    public MainPage(LoginPage loginPage, ITokenProvider tokenProvider)
    {
        InitializeComponent();
        _loginPage = loginPage;
        _tokenProvider = tokenProvider;
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
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await UpdateButtonStates();
    }
    
    private async Task UpdateButtonStates()
    {
        var token = await _tokenProvider.GetTokenAsync();
        bool isAuthorized = !string.IsNullOrEmpty(token);

        ScanBtn.IsEnabled = isAuthorized;
        SettingsBtn.IsEnabled = isAuthorized;
    }

    private void OnTestClicked(object sender, EventArgs e)
    {
        _tokenProvider.ClearToken();
    }
}