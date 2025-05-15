using Microsoft.Extensions.Logging;
using BarcodeScanner.Mobile;
using CommunityToolkit.Maui;
using MauiAndroid.App.Data.Services;
using MauiAndroid.App.Data.Utils;
using MauiAndroid.App.Handlers;
using MauiAndroid.App.Models;
using MauiAndroid.App.Pages;

#if ANDROID
using Android.Content.Res;
using Android.Graphics;
using Microsoft.Maui.Platform;
#endif

namespace MauiAndroid.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddBarcodeScannerHandler();
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ITokenProvider, TokenProvider>();
        builder.Services.AddTransient<JwtAuthorizationHandler>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<IHttpMessagingService, HttpMessagingService>();

        builder.Services.AddHttpClient("AnonymousClient")
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

        builder.Services.AddHttpClient("AuthorizedClient")
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
            .AddHttpMessageHandler<JwtAuthorizationHandler>();


#if ANDROID
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);

            handler.PlatformView.BackgroundTintList =
                ColorStateList.ValueOf(Microsoft.Maui.Graphics.Colors.Transparent.ToPlatform());
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
