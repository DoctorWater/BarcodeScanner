using Microsoft.Extensions.Logging;
using BarcodeScanner.Mobile;
using CommunityToolkit.Maui;

#if ANDROID
using Android.Content.Res;               // ColorStateList
using Android.Graphics;                  // Android.Graphics.Color
using Microsoft.Maui.Platform;           // ToPlatform()
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

        // DI‑контейнер
        builder.Services.AddScoped<BarcodeService>();

#if ANDROID
        // Убираем внутреннее подчёркивание у Entry только на Android
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            // Снимаем системный фон
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);

            // Прозрачная tint‑линия (акцентная линия)
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
