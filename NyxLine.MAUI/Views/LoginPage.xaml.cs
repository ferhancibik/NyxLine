using NyxLine.MAUI.Services;
using System.ComponentModel;

namespace NyxLine.MAUI.Views;

public partial class LoginPage : ContentPage, INotifyPropertyChanged
{
    private readonly IAuthService _authService;
    private bool _isBusy;
    private string _emailOrUsername = string.Empty;
    private string _password = string.Empty;
    public new bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    public bool IsNotBusy => !IsBusy;

    public string EmailOrUsername
    {
        get => _emailOrUsername;
        set
        {
            _emailOrUsername = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public LoginPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        BindingContext = this;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailOrUsername) || string.IsNullOrWhiteSpace(Password))
        {
            await DisplayAlert("Hata", "Lütfen tüm alanları doldurun", "Tamam");
            return;
        }

        IsBusy = true;

        try
        {
            var success = await _authService.LoginAsync(EmailOrUsername, Password);
            if (success)
            {
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                await DisplayAlert("Hata", "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnGuestLoginClicked(object sender, EventArgs e)
    {
        IsBusy = true;

        try
        {
            // Önceden tanımlı hayalet kullanıcı ile giriş yap
            var success = await _authService.LoginAsync("ghost", "Ghost123!");
            if (success)
            {
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                await DisplayAlert("Hata", "Misafir girişi başarısız. Lütfen tekrar deneyin.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("register");
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("forgotpassword");
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
} 