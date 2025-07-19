using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NyxLine.API.Data;
using NyxLine.API.DTOs;
using NyxLine.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NyxLine.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, User? User)> RegisterAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registrationDto.Email);
                if (existingUser != null)
                {
                    return (false, "Bu e-posta adresi zaten kullanılıyor", null);
                }

                existingUser = await _userManager.FindByNameAsync(registrationDto.UserName);
                if (existingUser != null)
                {
                    return (false, "Bu kullanıcı adı zaten kullanılıyor", null);
                }

                var user = new User
                {
                    UserName = registrationDto.UserName,
                    Email = registrationDto.Email,
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Bio = registrationDto.Bio,
                    IsGhost = registrationDto.IsGhost,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = registrationDto.UserName
                };

                var result = await _userManager.CreateAsync(user, registrationDto.Password);

                if (result.Succeeded)
                {
                    return (true, "Kullanıcı başarıyla oluşturuldu", user);
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Kullanıcı oluşturulamadı: {errors}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, string? Token, User? User)> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                User? user = null;

                // Try to find user by email or username
                if (loginDto.EmailOrUserName.Contains("@"))
                {
                    user = await _userManager.FindByEmailAsync(loginDto.EmailOrUserName);
                }
                else
                {
                    user = await _userManager.FindByNameAsync(loginDto.EmailOrUserName);
                }

                if (user == null)
                {
                    return (false, "Bu kullanıcı adı veya e-posta ile kayıtlı bir hesap bulunamadı. Önce kayıt olmalısınız.", null, null);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(user);
                    return (true, "Giriş başarılı", token, user);
                }

                return (false, "Şifre hatalı. Lütfen şifrenizi kontrol edin.", null, null);
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}", null, null);
            }
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (false, "Kullanıcı bulunamadı");
                }

                // Mevcut şifrenin doğruluğunu kontrol et
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    return (false, "Mevcut şifre yanlış");
                }

                // Yeni şifrenin mevcut şifreyle aynı olup olmadığını kontrol et
                if (changePasswordDto.CurrentPassword == changePasswordDto.NewPassword)
                {
                    return (false, "Yeni şifre mevcut şifreyle aynı olamaz");
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    user.UpdatedBy = user.UserName;
                    await _userManager.UpdateAsync(user);
                    
                    return (true, "Şifre başarıyla değiştirildi");
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Şifre değiştirilemedi: {errors}");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, string? Token)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                {
                    // Güvenlik nedeniyle e-posta bulunamasa bile başarılı mesajı döndürüyoruz
                    return (true, "Eğer bu e-posta kayıtlıysa, şifre sıfırlama bağlantısı gönderildi", null);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // Şimdilik sadece konsola yazdır (e-posta servisi sorun çıkarıyor)
                Console.WriteLine($"=== ŞİFRE SIFIRLAMA TOKEN'I ===");
                Console.WriteLine($"E-posta: {user.Email}");
                Console.WriteLine($"Token: {token}");
                Console.WriteLine($"Bu token'ı şifre sıfırlama sayfasında kullanın.");
                Console.WriteLine($"==============================");

                return (true, "Şifre sıfırlama token'ı oluşturuldu. Konsol çıktısını kontrol edin.", token);
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    return (false, "Bu e-posta ile kayıtlı kullanıcı bulunamadı");
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
                
                if (result.Succeeded)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    user.UpdatedBy = user.UserName;
                    await _userManager.UpdateAsync(user);
                    
                    return (true, "Şifre başarıyla sıfırlandı");
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Şifre sıfırlanamadı: {errors}");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task LogoutAsync(string userId)
        {
            await _signInManager.SignOutAsync();
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "YourVeryLongSecretKeyThatIsAtLeast32CharactersLong"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("IsGhost", user.IsGhost.ToString()),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"] ?? "NyxLineAPI",
                audience: jwtSettings["Audience"] ?? "NyxLineClient",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 