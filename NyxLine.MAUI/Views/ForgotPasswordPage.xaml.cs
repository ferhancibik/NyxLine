using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.ComponentModel;

namespace NyxLine.MAUI.Views;

public partial class ForgotPasswordPage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private bool _isBusy;
    private string _email = string.Empty;

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

    public ForgotPasswordPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        BindingContext = this;
    }

    private async void OnSendResetLinkClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            await DisplayAlert("Hata", "Lütfen e-posta adresinizi girin", "Tamam");
            return;
        }

        if (!Email.Contains("@"))
        {
            await DisplayAlert("Hata", "Lütfen geçerli bir e-posta adresi girin", "Tamam");
            return;
        }

        IsBusy = true;

        try
        {
            var request = new ForgotPasswordRequest
            {
                Email = Email
            };

            var result = await _apiService.ForgotPasswordAsync(request);
            if (result != null && !string.IsNullOrEmpty(result.Message))
            {
                if (result.Message.Contains("gönderildi") || result.Message.Contains("oluşturuldu") || result.Message.Contains("başarılı"))
                {
                    string message;
                    if (result.Message.Contains("gönderildi"))
                    {
                        // E-posta başarıyla gönderildi
                        message = $"📧 Şifre sıfırlama bağlantısı e-posta adresinize gönderildi!\n\n" +
                                $"E-postanızı kontrol edin ve bağlantıya tıklayarak şifrenizi sıfırlayın.\n\n" +
                                $"E-posta gelmezse spam klasörünü de kontrol edin.";
                    }
                    else
                    {
                        // E-posta gönderilemedi, token konsola yazdırıldı
                        message = $"📝 Şifre sıfırlama token'ı oluşturuldu!\n\n" +
                                $"E-posta gönderilemediği için token API konsolunda görüntülendi.\n\n" +
                                $"🔐 API konsoluna bakın ve token'ı kopyalayın.\n\n" +
                                $"Şifre sıfırlama sayfasına yönlendiriliyorsunuz...";
                    }
                    
                    await DisplayAlert("Başarılı", message, "Tamam");
                    
                    // E-posta ile token geldiyse, kullanıcı e-postadaki linke tıklayacak
                    // Token konsola yazdırıldıysa şifre sıfırlama sayfasına yönlendir
                    if (!result.Message.Contains("gönderildi"))
                    {
                        // Email'i kaydet ki ResetPasswordPage'de kullanabilelim
                        Preferences.Set("ResetPasswordEmail", Email);
                        
                        await Shell.Current.GoToAsync("//resetpassword");
                    }
                }
                else
                {
                    await DisplayAlert("Bilgi", result.Message, "Tamam");
                }
            }
            else
            {
                await DisplayAlert("Hata", 
                    "API'den yanıt alınamadı.\n\n" +
                    "✅ API'nin çalıştığından emin olun\n" +
                    "✅ Internet bağlantınızı kontrol edin", 
                    "Tamam");
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