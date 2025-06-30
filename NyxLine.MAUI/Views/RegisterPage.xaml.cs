using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.ComponentModel;

namespace NyxLine.MAUI.Views;

public partial class RegisterPage : ContentPage, INotifyPropertyChanged
{
    private readonly IAuthService _authService;
    private bool _isBusy;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _userName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _bio = string.Empty;

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

    public string FirstName
    {
        get => _firstName;
        set { _firstName = value; OnPropertyChanged(); }
    }

    public string LastName
    {
        get => _lastName;
        set { _lastName = value; OnPropertyChanged(); }
    }

    public string UserName
    {
        get => _userName;
        set { _userName = value; OnPropertyChanged(); }
    }

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public string Bio
    {
        get => _bio;
        set { _bio = value; OnPropertyChanged(); }
    }

    public RegisterPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        BindingContext = this;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            await DisplayAlert("Hata", "Lütfen zorunlu alanları doldurun", "Tamam");
            return;
        }

        if (Password.Length < 6)
        {
            await DisplayAlert("Hata", "Şifre en az 6 karakter olmalıdır", "Tamam");
            return;
        }

        IsBusy = true;

        try
        {
            var request = new RegisterRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                UserName = UserName,
                Email = Email,
                Password = Password,
                Bio = Bio,
                IsGhost = false
            };

            var result = await _authService.RegisterAsync(request);
            if (result.Success)
            {
                await DisplayAlert("Başarılı", "Kayıt işlemi tamamlandı. Şimdi giriş yapabilirsiniz.", "Tamam");
                await Shell.Current.GoToAsync("//login");
            }
            else
            {
                await DisplayAlert("Hata", result.Message, "Tamam");
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

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//login");
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
} 