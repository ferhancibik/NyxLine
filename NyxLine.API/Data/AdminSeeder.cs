using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NyxLine.API.Models;

namespace NyxLine.API.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            
            // Admin kullanıcısını bul
            var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
            
            if (adminUser != null && !adminUser.IsAdmin)
            {
                adminUser.IsAdmin = true;
                await userManager.UpdateAsync(adminUser);
                Console.WriteLine("Admin kullanıcısı başarıyla admin yapıldı!");
            }
            else if (adminUser == null)
            {
                Console.WriteLine("Admin kullanıcısı bulunamadı!");
            }
            else
            {
                Console.WriteLine("Kullanıcı zaten admin!");
            }
        }
    }
} 