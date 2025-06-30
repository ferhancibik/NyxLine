using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.ComponentModel;

namespace NyxLine.MAUI.Views;

public partial class CreatePostPage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private bool _isBusy;
    private string _content = string.Empty;
    private FileResult? _selectedImageFile;
    private const int MaxCharacters = 500;

    public new bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotBusy));
            UpdatePostButtonState();
        }
    }

    public bool IsNotBusy => !IsBusy;

    public new string Content
    {
        get => _content;
        set 
        { 
            _content = value; 
            OnPropertyChanged();
            UpdatePostButtonState();
        }
    }

    public CreatePostPage(IApiService apiService, IAuthService authService)
    {
        InitializeComponent();
        _apiService = apiService;
        _authService = authService;
        BindingContext = this;
        LoadUserInfo();
    }

    private async void LoadUserInfo()
    {
        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null)
            {
                UserNameLabel.Text = $"{currentUser.FirstName} {currentUser.LastName}";
                if (!string.IsNullOrEmpty(currentUser.ProfileImagePath))
                {
                    UserProfileImage.Source = $"http://localhost:8080{currentUser.ProfileImagePath}";
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Kullanıcı bilgileri yüklenirken hata: {ex.Message}");
        }
    }

    private void OnContentChanged(object sender, TextChangedEventArgs e)
    {
        var characterCount = e.NewTextValue?.Length ?? 0;
        CharacterCountLabel.Text = $"{characterCount}/{MaxCharacters}";
        
        // Karakter limiti kontrolü
        if (characterCount > MaxCharacters)
        {
            CharacterCountLabel.TextColor = Colors.Red;
            ContentEditor.Text = e.NewTextValue?[..MaxCharacters] ?? string.Empty;
        }
        else if (characterCount > MaxCharacters * 0.8) // %80'e ulaştığında uyarı rengi
        {
            CharacterCountLabel.TextColor = Colors.Orange;
        }
        else
        {
            CharacterCountLabel.TextColor = Colors.Gray;
        }
        
        Content = ContentEditor.Text;
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Bir resim seçin"
            });

            if (result != null)
            {
                _selectedImageFile = result;
                var stream = await result.OpenReadAsync();
                SelectedImage.Source = ImageSource.FromStream(() => stream);
                
                // UI güncellemesi
                UploadButton.IsVisible = false;
                ImagePreviewArea.IsVisible = true;
                
                UpdatePostButtonState();
                
                // Başarı animasyonu simülasyonu
                await ImagePreviewArea.FadeTo(0);
                ImagePreviewArea.IsVisible = true;
                await ImagePreviewArea.FadeTo(1, 300);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Resim seçimi başarısız: {ex.Message}", "Tamam");
        }
    }

    private async void OnRemoveImageClicked(object sender, EventArgs e)
    {
        try
        {
            _selectedImageFile = null;
            SelectedImage.Source = null;
            
            // UI güncellemesi animasyonla
            await ImagePreviewArea.FadeTo(0, 200);
            ImagePreviewArea.IsVisible = false;
            UploadButton.IsVisible = true;
            await UploadButton.FadeTo(1, 300);
            
            UpdatePostButtonState();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Resim kaldırılırken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private void UpdatePostButtonState()
    {
        var hasContent = !string.IsNullOrWhiteSpace(Content);
        var hasImage = _selectedImageFile != null;
        var isValidLength = Content.Length <= MaxCharacters;
        
        PostButton.IsEnabled = IsNotBusy && (hasContent || hasImage) && isValidLength;
        
        // Buton metnini duruma göre güncelle
        if (IsBusy)
        {
            PostButton.Text = "📤 Gönderiliyor...";
        }
        else if (!hasContent && !hasImage)
        {
            PostButton.Text = "📝 İçerik veya fotoğraf ekleyin";
        }
        else if (!isValidLength)
        {
            PostButton.Text = "⚠️ Metin çok uzun";
        }
        else
        {
            PostButton.Text = "📝 Gönderisini Paylaş";
        }
    }

    private async void OnPostClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Content) && _selectedImageFile == null)
        {
            await DisplayAlert("Uyarı", "Lütfen bir içerik yazın veya fotoğraf ekleyin", "Tamam");
            return;
        }

        if (Content.Length > MaxCharacters)
        {
            await DisplayAlert("Uyarı", $"Metin {MaxCharacters} karakterden uzun olamaz", "Tamam");
            return;
        }

        IsBusy = true;

        try
        {
            var request = new CreatePostRequest
            {
                Content = Content ?? string.Empty
            };

            // Resim varsa base64'e çevir
            if (_selectedImageFile != null)
            {
                using var stream = await _selectedImageFile.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                request.ImageBase64 = Convert.ToBase64String(imageBytes);
                request.FileName = _selectedImageFile.FileName;
            }

            var response = await _apiService.CreatePostAsync(request);
            if (response != null)
            {
                // Başarı animasyonu
                PostButton.Text = "✅ Paylaşıldı!";
                PostButton.BackgroundColor = Colors.Green;
                
                await Task.Delay(1000);
                
                await DisplayAlert("Başarılı", "Gönderi başarıyla paylaşıldı! 🎉", "Tamam");
                
                // Formu temizle
                Content = string.Empty;
                ContentEditor.Text = string.Empty;
                CharacterCountLabel.Text = "0/500";
                _selectedImageFile = null;
                SelectedImage.Source = null;
                ImagePreviewArea.IsVisible = false;
                UploadButton.IsVisible = true;
                
                // Ana sayfaya git
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                await DisplayAlert("Hata", "Gönderi paylaşılamadı. Lütfen tekrar deneyin.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            IsBusy = false;
            PostButton.BackgroundColor = null; // Varsayılan renge döndür
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Sayfa açılış animasyonu
        _ = Task.Run(() =>
        {
            Task.Delay(100).Wait();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ContentEditor.Focus();
            });
        });
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
} 