using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiAndroid.App.Data.Services;

namespace MauiAndroid.App.Models;
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly AuthService _authService;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        LoginCommand = new Command(async () => await LoginAsync());
    }

    private string _username = "";
    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(); }
    }

    private string _password = "";
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        ErrorMessage = "";

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Введите логин и пароль.";
            return;
        }

        var success = await _authService.LoginAsync(Username, Password);
        if (success)
        {
            MessagingCenter.Send(this, "LoginSuccess");
        }
        else
        {
            ErrorMessage = "Неверный логин или пароль.";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}