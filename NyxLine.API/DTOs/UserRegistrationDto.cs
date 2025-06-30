using System.ComponentModel.DataAnnotations;

namespace NyxLine.API.DTOs
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [MaxLength(100, ErrorMessage = "Kullanıcı adı en fazla 100 karakter olabilir")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur")]
        [MaxLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [MaxLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Biyografi en fazla 500 karakter olabilir")]
        public string? Bio { get; set; }

        public bool IsGhost { get; set; } = false;
    }
} 