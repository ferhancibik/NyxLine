using NyxLine.MAUI.Views;

namespace NyxLine.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Auth Routes
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("forgotpassword", typeof(ForgotPasswordPage));
        Routing.RegisterRoute("resetpassword", typeof(ResetPasswordPage));
        
        // Profile Routes
        Routing.RegisterRoute("changepassword", typeof(ChangePasswordPage));
        Routing.RegisterRoute("userprofile", typeof(UserProfilePage));
        Routing.RegisterRoute("editprofile", typeof(EditProfilePage));
        
        // Star Routes
        Routing.RegisterRoute("stardetail", typeof(StarDetailPage));
    }
}
