using MauiAndroid.App.Models;
using Microsoft.Maui.Controls;

namespace MauiAndroid.App.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            MessagingCenter.Subscribe<LoginViewModel>(this, "LoginSuccess", async vm =>
            {
                await DisplayAlert("Успех", "Вы успешно авторизовались.", "OK");
                await Navigation.PopAsync();
            });
        }
    }
}
