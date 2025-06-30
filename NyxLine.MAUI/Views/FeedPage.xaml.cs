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
    private ObservableCollection<Post> _posts = new();
    private bool _isRefreshing;
    private bool _isBusy;

    public ObservableCollection<Post> Posts
    {
        get => _posts;
        set { _posts = value; OnPropertyChanged(); }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; OnPropertyChanged(); }
    }

    public new bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    public ICommand RefreshCommand { get; }

    public FeedPage(IApiService apiService, IAuthService authService)
    {
        InitializeComponent();
        _apiService = apiService;
        _authService = authService;
        BindingContext = this;
        RefreshCommand = new Command(async () => await RefreshPostsAsync());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("🔄 FeedPage: OnAppearing - Sayfa açılıyor...");
        
        // Her sayfaya geldiğinde fresh data yükle
        await RefreshPostsAsync();
    }

    private async Task LoadPostsAsync()
    {
        if (IsBusy) 
        {
            System.Diagnostics.Debug.WriteLine("⚠️ LoadPostsAsync: Zaten yükleniyor, atlanıyor...");
            return;
        }

        IsBusy = true;
        try
        {
            System.Diagnostics.Debug.WriteLine("📡 API'den gönderiler yükleniyor...");
            var posts = await _apiService.GetFeedAsync();
            
            if (posts != null && posts.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"📦 API'den {posts.Count} gönderi geldi");
                
                // Debug: Her post için detayları yazdır
                foreach (var post in posts)
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Post #{post.Id}: User={post.UserFullName}, Content='{post.Content}', ImagePath='{post.ImagePath}', PostImageUrl='{post.PostImageUrl}'");
                }

                // UI thread'de güncellemeleri yap
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Posts.Clear();
                    foreach (var post in posts)
                    {
                        Posts.Add(post);
                    }
                });
                
                System.Diagnostics.Debug.WriteLine($"✅ Feed başarıyla yüklendi: {Posts.Count} gönderi UI'ye eklendi");
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Posts.Clear();
                });
                System.Diagnostics.Debug.WriteLine("ℹ️ Feed boş - henüz gönderi yok");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Feed yükleme hatası: {ex.Message}");
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Hata", $"Gönderiler yüklenemedi: {ex.Message}", "Tamam");
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshPostsAsync()
    {
        System.Diagnostics.Debug.WriteLine("🔄 Feed yenileniyor...");
        IsRefreshing = true;
        try
        {
            await LoadPostsAsync();
            System.Diagnostics.Debug.WriteLine("✅ Feed yenileme tamamlandı!");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Feed yenileme hatası: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    // YENİ: Manuel yenileme butonu için
    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("🔄 Manuel yenileme butonu tıklandı");
        await RefreshPostsAsync();
        
        // Kullanıcıya geri bildirim ver
        await DisplayAlert("Yenilendi", "Feed başarıyla yenilendi! 📱", "Tamam");
    }

    private async void OnLikeClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Post post)
        {
            try
            {
                if (post.IsLikedByCurrentUser)
                {
                    await _apiService.UnlikePostAsync(post.Id);
                    post.IsLikedByCurrentUser = false;
                    post.LikeCount--;
                }
                else
                {
                    await _apiService.LikePostAsync(post.Id);
                    post.IsLikedByCurrentUser = true;
                    post.LikeCount++;
                }
                
                System.Diagnostics.Debug.WriteLine($"💖 Gönderi #{post.Id} beğeni durumu güncellendi: {post.IsLikedByCurrentUser}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Beğeni işlemi başarısız: {ex.Message}", "Tamam");
            }
        }
    }

    private async void OnNotificationsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Bildirimler", "Henüz yeni bildirim yok! 🔔", "Tamam");
    }

    private async void OnMoreOptionsClicked(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is Post post)
        {
            var action = await DisplayActionSheet("Seçenekler", "İptal", null, "Şikayet Et", "Engelle", "Detay Görüntüle", "Yenile");
            
            switch (action)
            {
                case "Şikayet Et":
                    await DisplayAlert("Şikayet", "Gönderi şikayet edildi. 🚨", "Tamam");
                    break;
                case "Engelle":
                    await DisplayAlert("Engelle", "Kullanıcı engellendi. 🚫", "Tamam");
                    break;
                case "Detay Görüntüle":
                    await DisplayAlert("Post Detayları", 
                        $"ID: {post.Id}\n" +
                        $"User: {post.UserFullName}\n" +
                        $"Content: {post.Content}\n" +
                        $"ImagePath: {post.ImagePath}\n" +
                        $"PostImageUrl: {post.PostImageUrl}\n" +
                        $"Likes: {post.LikeCount}\n" +
                        $"Created: {post.CreatedAt}", "Tamam");
                    break;
                case "Yenile":
                    await RefreshPostsAsync();
                    break;
            }
        }
    }

    private async void OnCommentClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Post post)
        {
            await DisplayAlert("Yorumlar", $"'{post.UserFullName}' gönderisine yorum yapabilirsiniz. 💬\n\n(Yakında bu özellik eklenecek!)", "Tamam");
        }
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Post post)
        {
            try
            {
                var shareText = $"NyxLine'da {post.UserFullName} tarafından paylaşılan gönderi:\n\n{post.Content}";
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = "NyxLine Gönderisi Paylaş"
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Paylaşım başarısız: {ex.Message}", "Tamam");
            }
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        base.OnPropertyChanged(propertyName);
    }
} 