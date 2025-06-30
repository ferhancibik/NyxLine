using System.ComponentModel.DataAnnotations;

namespace NyxLine.API.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "E-posta veya kullanıcı adı zorunludur")]
        public string EmailOrUserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; } = string.Empty;
    }
} 