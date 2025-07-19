using Microsoft.Extensions.Logging;
using NyxLine.MAUI.Services;
using NyxLine.MAUI.Views;

namespace NyxLine.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Quicksand-Regular.ttf", "QuicksandRegular");
                fonts.AddFont("Quicksand-Light.ttf", "QuicksandLight");
                fonts.AddFont("Quicksand-Medium.ttf", "QuicksandMedium");
                fonts.AddFont("Quicksand-SemiBold.ttf", "QuicksandSemiBold");
                fonts.AddFont("Quicksand-Bold.ttf", "QuicksandBold");
            })
            .ConfigureEssentials(essentials =>
            {
                essentials.UseVersionTracking();
            })
            .ConfigureMauiHandlers(handlers =>
            {
                #if WINDOWS
                handlers.AddHandler<Microsoft.Maui.Controls.Image, Microsoft.Maui.Handlers.ImageHandler>();
                #endif
            });

        // Add HttpClient
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<HttpClient>();

        // Register services
        builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<FeedPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<CreatePostPage>();
        builder.Services.AddTransient<ChangePasswordPage>();
        builder.Services.AddTransient<ForgotPasswordPage>();
        builder.Services.AddTransient<ResetPasswordPage>();
        builder.Services.AddTransient<UserProfilePage>();
        builder.Services.AddTransient<EditProfilePage>();
        builder.Services.AddTransient<NewsPage>();
        builder.Services.AddTransient<AstrologyPage>();

#if DEBUG
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        return builder.Build();
    }
}
