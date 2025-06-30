using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace NyxLine.MAUI.Views;

[QueryProperty(nameof(UserId), "userId")]
public partial class UserProfilePage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private string _userId = string.Empty;
    private User? _user;
    private ObservableCollection<Post> _userPosts = new();
    private bool _isLoading;
    private bool _isRefreshing;

    public string UserId
    {
        get => _userId;
        set
        {
            _userId = value;
            OnPropertyChanged();
            if (!string.IsNullOrEmpty(value))
            {
                _ = LoadUserProfileAsync();
            }
        }
    }

    public User? User
    {
        get => _user;
        set
        {
            _user = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasPosts));
            OnPropertyChanged(nameof(HasNoPosts));
        }
    }

    public ObservableCollection<Post> UserPosts
    {
        get => _userPosts;
        set
        {
            _userPosts = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasPosts));
            OnPropertyChanged(nameof(HasNoPosts));
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged();
        }
    }

    public bool HasPosts => UserPosts?.Count > 0 && !IsLoading;
    public bool HasNoPosts => UserPosts?.Count == 0 && !IsLoading;

    public ICommand RefreshCommand { get; }
    public ICommand ToggleFollowCommand { get; }

    public UserProfilePage(IApiService apiService, IAuthService authService)
    {
        InitializeComponent();
        _apiService = apiService;
        _authService = authService;
        BindingContext = this;
        
        RefreshCommand = new Command(async () => await RefreshAsync());
        ToggleFollowCommand = new Command(async () => await ToggleFollowAsync());
    }

    private async Task LoadUserProfileAsync()
    {
        if (string.IsNullOrEmpty(UserId))
            return;

        try
        {
            IsLoading = true;

            // Kullanıcı profilini yükle
            var userProfile = await _apiService.GetUserProfileAsync(UserId);
            if (userProfile != null)
            {
                User = userProfile;
            }

            // Kullanıcının gönderilerini yükle
            await LoadUserPostsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Profil yüklenemedi: {ex.Message}", "Tamam");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadUserPostsAsync()
    {
        try
        {
            var posts = await _apiService.GetUserPostsAsync(UserId, 0, 20);
            if (posts != null)
            {
                UserPosts.Clear();
                foreach (var post in posts)
                {
                    UserPosts.Add(post);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Gönderiler yüklenemedi: {ex.Message}", "Tamam");
        }
    }

    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadUserProfileAsync();
        IsRefreshing = false;
    }

    private async Task ToggleFollowAsync()
    {
        if (User == null)
            return;

        try
        {
            if (User.IsFollowedByCurrentUser)
            {
                var result = await _apiService.UnfollowUserAsync(User.Id);
                if (result != null)
                {
                    User.IsFollowedByCurrentUser = false;
                    User.FollowersCount--;
                    OnPropertyChanged(nameof(User));
                }
            }
            else
            {
                var result = await _apiService.FollowUserAsync(User.Id);
                if (result != null)
                {
                    User.IsFollowedByCurrentUser = true;
                    User.FollowersCount++;
                    OnPropertyChanged(nameof(User));
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Takip işlemi başarısız: {ex.Message}", "Tamam");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
} 