using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace NyxLine.MAUI.Views;

public partial class EditProfilePage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private bool _isBusy;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string? _bio;
    private ImageSource? _profileImageSource;
    private string? _selectedImageBase64;

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

    public string? Bio
    {
        get => _bio;
        set { _bio = value; OnPropertyChanged(); }
    }

    public ImageSource? ProfileImageSource
    {
        get => _profileImageSource;
        set { _profileImageSource = value; OnPropertyChanged(); }
    }

    // Parametresiz constructor - MAUI routing için gerekli
    public EditProfilePage()
    {
        InitializeComponent();
        
        // Service provider'dan dependencies'leri al
        var serviceProvider = Application.Current?.Handler?.MauiContext?.Services;
        if (serviceProvider != null)
        {
            _apiService = serviceProvider.GetRequiredService<IApiService>();
            _authService = serviceProvider.GetRequiredService<IAuthService>();
        }
        else
        {
            throw new InvalidOperationException("Service provider bulunamadı");
        }
        
        BindingContext = this;
    }

    // Dependency injection constructor - eski kod için backward compatibility
    public EditProfilePage(IApiService apiService, IAuthService authService)
    {
        InitializeComponent();
        _apiService = apiService;
        _authService = authService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCurrentUserDataAsync();
    }

    private async Task LoadCurrentUserDataAsync()
    {
        try
        {
            IsBusy = true;
            var user = await _authService.GetCurrentUserAsync();
            if (user != null)
            {
                FirstName = user.FirstName;
                LastName = user.LastName;
                Bio = user.Bio;
                
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    ProfileImageSource = ImageSource.FromUri(new Uri(user.ProfileImageUrl));
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Kullanıcı bilgileri yüklenemedi: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Profil fotoğrafı seçin"
            });

            if (result != null)
            {
                // Seçilen fotoğrafı göster
                ProfileImageSource = ImageSource.FromStream(() => result.OpenReadAsync().Result);
                
                // Base64 formatına çevir
                using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                _selectedImageBase64 = Convert.ToBase64String(bytes);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Fotoğraf seçilemedi: {ex.Message}", "Tamam");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            await DisplayAlert("Hata", "Ad ve soyad alanları zorunludur", "Tamam");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new UpdateProfileRequest
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                Bio = string.IsNullOrWhiteSpace(Bio) ? null : Bio.Trim(),
                ProfileImageBase64 = _selectedImageBase64
            };

            var response = await _apiService.UpdateProfileAsync(request);
            
            if (response != null && (response.Message.Contains("başarıyla") || response.Message.Contains("tamamlandı")))
            {
                // Kullanıcı bilgilerini yenile
                await _authService.RefreshCurrentUserAsync();
                
                await DisplayAlert("Başarılı", response.Message, "Tamam");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Hata", response?.Message ?? "Profil güncellenemedi", "Tamam");
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

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Onay", "Değişiklikleri kaydetmeden çıkmak istediğinizden emin misiniz?", "Evet", "Hayır");
        if (confirm)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
}