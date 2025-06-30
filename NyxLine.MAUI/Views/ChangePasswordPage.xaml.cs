using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.ComponentModel;

namespace NyxLine.MAUI.Views;

public partial class ChangePasswordPage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private bool _isBusy;
    private string _currentPassword = string.Empty;
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

    public string CurrentPassword
    {
        get => _currentPassword;
        set { _currentPassword = value; OnPropertyChanged(); }
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

    public ChangePasswordPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        BindingContext = this;
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword) || 
            string.IsNullOrWhiteSpace(NewPassword) || 
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            await DisplayAlert("Hata", "Lütfen tüm alanları doldurun", "Tamam");
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            await DisplayAlert("Hata", "Yeni şifreler eşleşmiyor", "Tamam");
            return;
        }

        if (NewPassword.Length < 6)
        {
            await DisplayAlert("Hata", "Yeni şifre en az 6 karakter olmalıdır", "Tamam");
            return;
        }

        IsBusy = true;

        try
        {
            var request = new ChangePasswordRequest
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            };

            var response = await _apiService.ChangePasswordAsync(request);
            if (response != null)
            {
                await DisplayAlert("Başarılı", "Şifre başarıyla değiştirildi", "Tamam");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Hata", "Şifre değiştirilemedi", "Tamam");
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

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
} 