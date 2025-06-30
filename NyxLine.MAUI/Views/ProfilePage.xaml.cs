using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NyxLine.MAUI.Views;

public partial class ProfilePage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private User? _user;
    private ObservableCollection<Post> _myPosts = new();
    private bool _isRefreshing;
    private bool _isLoadingPosts;

    public User? User
    {
        get => _user;
        set { _user = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Post> MyPosts
    {
        get => _myPosts;
        set { _myPosts = value; OnPropertyChanged(); }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; OnPropertyChanged(); }
    }

    public bool IsLoadingPosts
    {
        get => _isLoadingPosts;
        set 
        { 
            _isLoadingPosts = value; 
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasPosts));
            OnPropertyChanged(nameof(HasNoPosts));
        }
    }

    public bool HasPosts => !IsLoadingPosts && MyPosts?.Count > 0;
    public bool HasNoPosts => !IsLoadingPosts && (MyPosts?.Count == 0 || MyPosts == null);

    public ProfilePage(IApiService apiService, IAuthService authService)
    {
        InitializeComponent();
        _apiService = apiService;
        _authService = authService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserProfileAsync();
        await LoadMyPostsAsync();
    }

    private async Task LoadUserProfileAsync()
    {
        try
        {
            // Profil fotoğrafının güncel gelmesi için cache'i yenile
            User = await _authService.RefreshCurrentUserAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Profil yüklenemedi: {ex.Message}", "Tamam");
        }
    }

    private async Task LoadMyPostsAsync()
    {
        try
        {
            IsLoadingPosts = true;
            
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser?.Id == null)
            {
                await DisplayAlert("Hata", "Kullanıcı bilgisi alınamadı", "Tamam");
                return;
            }

            var posts = await _apiService.GetUserPostsAsync(currentUser.Id, 1, 20);
            if (posts != null)
            {
                MyPosts.Clear();
                foreach (var post in posts)
                {
                    MyPosts.Add(post);
                }
            }
            else
            {
                await DisplayAlert("Hata", "Gönderiler yüklenemedi", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Gönderiler yüklenemedi: {ex.Message}", "Tamam");
        }
        finally
        {
            IsLoadingPosts = false;
        }
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        try
        {
            IsRefreshing = true;
            await LoadUserProfileAsync();
            await LoadMyPostsAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async void OnDeletePostClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            if (button?.CommandParameter is int postId)
            {
                var confirm = await DisplayAlert("Onay", 
                    "Bu gönderiyi silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.", 
                    "Evet, Sil", "İptal");
                
                if (confirm)
                {
                    var response = await _apiService.DeletePostAsync(postId);
                    if (response != null)
                    {
                        // Remove from local collection
                        var postToRemove = MyPosts.FirstOrDefault(p => p.Id == postId);
                        if (postToRemove != null)
                        {
                            MyPosts.Remove(postToRemove);
                        }

                        await DisplayAlert("Başarılı", "Gönderi başarıyla silindi", "Tamam");
                        
                        // Update user profile to refresh post count
                        await LoadUserProfileAsync();
                    }
                    else
                    {
                        await DisplayAlert("Hata", "Gönderi silinemedi", "Tamam");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Gönderi silinemedi: {ex.Message}", "Tamam");
        }
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("editprofile");
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("changepassword");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Onay", "Çıkış yapmak istediğinizden emin misiniz?", "Evet", "Hayır");
        if (confirm)
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
} 