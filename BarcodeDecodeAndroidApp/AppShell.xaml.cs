using MauiAndroid.App.Pages;

namespace MauiAndroid.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        InitializeComponent();
    }
}