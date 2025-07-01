using System.Collections.ObjectModel;
using System.Windows.Input;
using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;

namespace NyxLine.MAUI.Views
{
    public partial class NewsPage : ContentPage
    {
        private readonly IApiService _apiService;
        private readonly IAuthService _authService;
        private int _currentPage = 1;
        private bool _isLoading;
        private bool _isRefreshing;
        private ObservableCollection<Post> _news;
        private bool _isAdmin;

        public ObservableCollection<Post> News
        {
            get => _news;
            set
            {
                _news = value;
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
        public ICommand AddNewsCommand { get; }
        public ICommand DeletePostCommand { get; }

        public NewsPage(IApiService apiService, IAuthService authService)
        {
            InitializeComponent();
            _apiService = apiService;
            _authService = authService;

            News = new ObservableCollection<Post>();

            RefreshCommand = new Command(async () => await RefreshAsync());
            LoadMoreCommand = new Command(async () => await LoadMoreAsync());
            LikeCommand = new Command<Post>(async (post) => await LikePostAsync(post));
            AddNewsCommand = new Command(async () => await AddNewsAsync());
            DeletePostCommand = new Command<int>(async (postId) => await DeletePostAsync(postId));

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Admin kontrolü
            await CheckAdminStatusAsync();
            
            if (News.Count == 0)
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

                var news = await _apiService.GetNewsAsync(_currentPage);
                News.Clear();
                if (news != null)
                {
                    foreach (var item in news)
                    {
                        News.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", "Haberler yüklenirken bir hata oluştu", "Tamam");
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

                var news = await _apiService.GetNewsAsync(_currentPage);
                if (news != null)
                {
                    foreach (var item in news)
                    {
                        News.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", "Daha fazla haber yüklenirken bir hata oluştu", "Tamam");
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
        
        private async Task AddNewsAsync()
        {
            if (!IsAdmin)
            {
                await DisplayAlert("Hata", "Haber eklemek için admin yetkisine sahip olmalısınız", "Tamam");
                return;
            }

            // Haber başlığı
            string newsTitle = await DisplayPromptAsync("Haber Ekle", "Haber başlığı:", "Tamam", "İptal");
            if (string.IsNullOrWhiteSpace(newsTitle))
                return;

            // Haber içeriği
            string content = await DisplayPromptAsync("Haber Ekle", "Haber içeriği:", "Tamam", "İptal");
            if (string.IsNullOrWhiteSpace(content))
                return;

            try
            {
                // Resim seçme işlemi (opsiyonel)
                string? imageBase64 = null;
                string? fileName = null;

                bool addImage = await DisplayAlert("Resim Ekle", "Habere resim eklemek istiyor musunuz?", "Evet", "Hayır");
                if (addImage)
                {
                    var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Haber Görseli Seç"
                    });

                    if (result != null)
                    {
                        var stream = await result.OpenReadAsync();
                        var bytes = new byte[stream.Length];
                        await stream.ReadAsync(bytes, 0, (int)stream.Length);
                        imageBase64 = Convert.ToBase64String(bytes);
                        fileName = result.FileName;
                    }
                }

                // Haber oluşturma isteği
                var request = new CreatePostRequest
                {
                    NewsTitle = newsTitle,
                    Content = content,
                    ImageBase64 = imageBase64,
                    FileName = fileName,
                    Type = PostType.News
                };

                var response = await _apiService.CreateNewsAsync(request);
                if (response?.Post != null)
                {
                    await DisplayAlert("Başarılı", "Haber başarıyla eklendi", "Tamam");
                    await RefreshAsync();
                }
                else
                {
                    await DisplayAlert("Hata", "Haber eklenirken bir hata oluştu", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Haber eklenirken bir hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async Task DeletePostAsync(int postId)
        {
            if (!IsAdmin)
            {
                await DisplayAlert("Hata", "Haber silmek için admin yetkisine sahip olmalısınız", "Tamam");
                return;
            }

            bool confirm = await DisplayAlert("Onay", "Bu haberi silmek istediğinize emin misiniz?", "Evet", "Hayır");
            if (!confirm)
                return;

            try
            {
                var response = await _apiService.AdminDeletePostAsync(postId);
                if (response?.Message != null)
                {
                    await DisplayAlert("Başarılı", "Haber başarıyla silindi", "Tamam");
                    
                    // Silinen haberi listeden kaldır
                    var postToRemove = News.FirstOrDefault(p => p.Id == postId);
                    if (postToRemove != null)
                    {
                        News.Remove(postToRemove);
                    }
                }
                else
                {
                    await DisplayAlert("Hata", "Haber silinirken bir hata oluştu", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Haber silinirken bir hata oluştu: {ex.Message}", "Tamam");
            }
        }
    }
} 