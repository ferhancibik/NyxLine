using NyxLine.MAUI.Services;

namespace NyxLine.MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        // Check if user is logged in
        var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
        if (authService != null)
        {
            var isLoggedIn = await authService.IsLoggedInAsync();
            if (isLoggedIn)
            {
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                await Shell.Current.GoToAsync("//login");
            }
        }
        else
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
