namespace MauiAndroid.App.Pages;

public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            AddressEntry.Text = Preferences.Get("ServerAddress", "192.168.1.104");
            PortEntry.Text = Preferences.Get("ServerPort", "7214");
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AddressEntry.Text) || string.IsNullOrWhiteSpace(PortEntry.Text))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, заполните оба поля.", "OK");
                return;
            }

            Preferences.Set("ServerAddress", AddressEntry.Text);
            Preferences.Set("ServerPort", PortEntry.Text);

            await DisplayAlert("Настройки сохранены", "Адрес и порт сервера сохранены.", "OK");

            await Navigation.PopAsync();
        }
    }