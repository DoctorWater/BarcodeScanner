

namespace MauiAndroid.App;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    public App(IServiceProvider services, MainPage mainPage)
    {
        InitializeComponent();
        Services = services;

        MainPage = new NavigationPage(mainPage);
    }
}