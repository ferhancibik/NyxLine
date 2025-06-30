using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using NyxLine.MAUI.Models;
using NyxLine.MAUI.Services;

namespace NyxLine.MAUI.Views;

public partial class SearchPage : ContentPage, INotifyPropertyChanged
{
    private readonly IApiService _apiService;
    private string _searchText = "";
    private bool _isLoading = false;
    private ObservableCollection<User> _searchResults = new();

    public SearchPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        BindingContext = this;
        
        ToggleFollowCommand = new Command<string>(async (userId) => await ToggleFollow(userId));
    }

    public ObservableCollection<User> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
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

    public bool IsSearchResultsEmpty => !SearchResults.Any() && !IsLoading && !string.IsNullOrWhiteSpace(_searchText);

    public ICommand ToggleFollowCommand { get; }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            SearchResults.Clear();
            return;
        }

        SearchText = e.NewTextValue;
        
        // Debounce search
        await Task.Delay(500);
        
        if (SearchText == e.NewTextValue)
        {
            await SearchUsers(e.NewTextValue);
        }
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        await SearchUsers(SearchText);
    }

    private async void OnUserTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is User user)
        {
            await Shell.Current.GoToAsync($"userprofile?userId={user.Id}");
        }
    }

    private async Task SearchUsers(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return;

        try
        {
            IsLoading = true;
            
            var users = await _apiService.SearchUsersAsync(query);
            
            if (users != null)
            {
                SearchResults.Clear();
                foreach (var user in users)
                {
                    SearchResults.Add(user);
                }
            }
            else
            {
                SearchResults.Clear();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Arama sırasında bir hata oluştu: {ex.Message}", "Tamam");
            SearchResults.Clear();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ToggleFollow(string userId)
    {
        var user = SearchResults.FirstOrDefault(u => u.Id == userId);
        if (user == null) return;

        try
        {
            MessageResponse? response;
            
            if (user.IsFollowedByCurrentUser)
            {
                response = await _apiService.UnfollowUserAsync(userId);
            }
            else
            {
                response = await _apiService.FollowUserAsync(userId);
            }

            // MessageResponse'da IsSuccess yok, o yüzden message içeriğini kontrol edelim
            if (response != null && !string.IsNullOrEmpty(response.Message))
            {
                // Başarılı mesajları genellikle "başarı" kelimesini içerir
                if (response.Message.ToLower().Contains("başarı") || response.Message.ToLower().Contains("success"))
                {
                    user.IsFollowedByCurrentUser = !user.IsFollowedByCurrentUser;
                    
                    // UI güncellemesi için CollectionView'i yenile
                    var index = SearchResults.IndexOf(user);
                    if (index >= 0)
                    {
                        SearchResults.RemoveAt(index);
                        SearchResults.Insert(index, user);
                    }
                }
                else
                {
                    await DisplayAlert("Hata", response.Message, "Tamam");
                }
            }
            else
            {
                await DisplayAlert("Hata", "İşlem başarısız", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"İşlem sırasında bir hata oluştu: {ex.Message}", "Tamam");
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected virtual new void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 