using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace NyxLine.MAUI.Views;

public partial class FeedPage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private int _currentPage = 1;
    private bool _isLoading;
    private bool _isRefreshing;
    private ObservableCollection<Post> _posts;
    private bool _isAdmin;

    public ObservableCollection<Post> Posts
    {
        get => _posts;
        set
        {
            _posts = value;
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

    public bool IsAdmin
    {
        get => _isAdmin;
        set
        {
            _isAdmin = value;
            OnPropertyChanged();
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand LoadMoreCommand { get; }
    public ICommand LikeCommand { get; }
    public ICommand DeletePostCommand { get; }

    public FeedPage(IApiService apiService, IAuthService authService)
    {
        InitializeComponent();
        _apiService = apiService;
        _authService = authService;

        Posts = new ObservableCollection<Post>();

        RefreshCommand = new Command(async () => await RefreshAsync());
        LoadMoreCommand = new Command(async () => await LoadMoreAsync());
        LikeCommand = new Command<Post>(async (post) => await LikePostAsync(post));
        DeletePostCommand = new Command<int>(async (postId) => await DeletePostAsync(postId));

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Admin kontrolü
        await CheckAdminStatusAsync();
        
        if (Posts.Count == 0)
        {
            await RefreshAsync();
        }
    }

    private async Task CheckAdminStatusAsync()
    {
        try
        {
            var user = await _apiService.GetCurrentUserProfileAsync();
            IsAdmin = user?.IsAdmin ?? false;
        }
        catch (Exception ex)
        {
            IsAdmin = false;
        }
    }

    private async Task RefreshAsync()
    {
        if (_isLoading) return;

        try
        {
            IsRefreshing = true;
            _isLoading = true;
            _currentPage = 1;

            // Normal gönderileri yükle
            var posts = await _apiService.GetFeedAsync(_currentPage);
            Posts.Clear();
            if (posts != null)
            {
                foreach (var post in posts.Where(p => p.Type == PostType.Regular))
                {
                    Posts.Add(post);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Gönderiler yüklenirken bir hata oluştu", "Tamam");
        }
        finally
        {
            _isLoading = false;
            IsRefreshing = false;
        }
    }

    private async Task LoadMoreAsync()
    {
        if (_isLoading) return;

        try
        {
            _isLoading = true;
            _currentPage++;

            var posts = await _apiService.GetFeedAsync(_currentPage);
            if (posts != null)
            {
                foreach (var post in posts.Where(p => p.Type == PostType.Regular))
                {
                    Posts.Add(post);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Daha fazla gönderi yüklenirken bir hata oluştu", "Tamam");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task LikePostAsync(Post post)
    {
        try
        {
            var response = post.IsLikedByCurrentUser
                ? await _apiService.UnlikePostAsync(post.Id)
                : await _apiService.LikePostAsync(post.Id);

            if (response?.Message != null)
            {
                post.IsLikedByCurrentUser = !post.IsLikedByCurrentUser;
                post.LikeCount += post.IsLikedByCurrentUser ? 1 : -1;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Beğeni işlemi sırasında bir hata oluştu", "Tamam");
        }
    }

    private async Task DeletePostAsync(int postId)
    {
        if (!IsAdmin)
        {
            await DisplayAlert("Hata", "Gönderi silmek için admin yetkisine sahip olmalısınız", "Tamam");
            return;
        }

        bool confirm = await DisplayAlert("Onay", "Bu gönderiyi silmek istediğinize emin misiniz?", "Evet", "Hayır");
        if (!confirm)
            return;

        try
        {
            var response = await _apiService.AdminDeletePostAsync(postId);
            if (response?.Message != null)
            {
                await DisplayAlert("Başarılı", "Gönderi başarıyla silindi", "Tamam");
                
                // Silinen gönderiyi listeden kaldır
                var postToRemove = Posts.FirstOrDefault(p => p.Id == postId);
                if (postToRemove != null)
                {
                    Posts.Remove(postToRemove);
                }
            }
            else
            {
                await DisplayAlert("Hata", "Gönderi silinirken bir hata oluştu", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Gönderi silinirken bir hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await RefreshAsync();
        await DisplayAlert("Yenilendi", "Feed başarıyla yenilendi! ��", "Tamam");
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
} 