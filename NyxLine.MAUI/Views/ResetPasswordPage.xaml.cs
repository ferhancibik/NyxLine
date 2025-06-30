using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.ComponentModel;

namespace NyxLine.MAUI.Views;

public partial class ResetPasswordPage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private bool _isBusy;
    private string _email = string.Empty;
    private string _token = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;

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

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    public string Token
    {
        get => _token;
        set { _token = value; OnPropertyChanged(); }
    }

    public string NewPassword
    {
        get => _newPassword;
        set { _newPassword = value; OnPropertyChanged(); }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set { _confirmPassword = value; OnPropertyChanged(); }
    }

    public ResetPasswordPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // ForgotPasswordPage'den geliyorsak email'i doldur
        var savedEmail = Preferences.Get("ResetPasswordEmail", string.Empty);
        if (!string.IsNullOrEmpty(savedEmail))
        {
            Email = savedEmail;
        }
    }

    private async void OnResetPasswordClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            await DisplayAlert("Hata", "Lütfen e-posta adresinizi girin", "Tamam");
            return;
        }

        if (string.IsNullOrWhiteSpace(Token))
        {
            await DisplayAlert("Hata", "Lütfen sıfırlama token'ını girin", "Tamam");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            await DisplayAlert("Hata", "Lütfen yeni şifrenizi girin", "Tamam");
            return;
        }

        if (NewPassword.Length < 3)
        {
            await DisplayAlert("Hata", "Şifre en az 3 karakter olmalıdır", "Tamam");
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            await DisplayAlert("Hata", "Şifreler eşleşmiyor", "Tamam");
            return;
        }

        IsBusy = true;

        try
        {
            var request = new ResetPasswordRequest
            {
                Email = Email,
                Token = Token,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            };

            var result = await _apiService.ResetPasswordAsync(request);
            if (result != null)
            {
                if (result.Message.Contains("başarılı") || result.Message.Contains("sıfırlandı"))
                {
                    await DisplayAlert("Başarılı", 
                        "Şifreniz başarıyla sıfırlandı. Şimdi yeni şifrenizle giriş yapabilirsiniz.", 
                        "Tamam");
                    
                    // Login sayfasına yönlendir
                    await Shell.Current.GoToAsync("//login");
                }
                else
                {
                    await DisplayAlert("Hata", result.Message, "Tamam");
                }
            }
            else
            {
                await DisplayAlert("Hata", "Bir hata oluştu. Lütfen tekrar deneyin.", "Tamam");
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

    private async void OnBackToLoginTapped(object sender, EventArgs e)
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