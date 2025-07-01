using NyxLine.MAUI.Views;

namespace NyxLine.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("main", typeof(FeedPage));
        Routing.RegisterRoute("profile", typeof(ProfilePage));
        Routing.RegisterRoute("search", typeof(SearchPage));
        Routing.RegisterRoute("createpost", typeof(CreatePostPage));
        Routing.RegisterRoute("changepassword", typeof(ChangePasswordPage));
        Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("resetpassword", typeof(ResetPasswordPage));
        Routing.RegisterRoute("userprofile", typeof(UserProfilePage));
        Routing.RegisterRoute("editprofile", typeof(EditProfilePage));
        Routing.RegisterRoute("news", typeof(NewsPage));
    }
}
