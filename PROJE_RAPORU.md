# Isparta Uygulamalı Bilimler Üniveristesi
# TEKNOLOJİ FAKÜLTESİ
# BİLGİSAYAR MÜHENDİSLİĞİ BÖLÜMÜ

# BİLGİSAYAR PROGRAMLAMA-II DERSİ
# PROJE RAPORU

## Proje Adı: NyxLine - Astronomi Sosyal Medya Platformu

### Hazırlayan
[ÖĞRENCİ ADI SOYADI]
[ÖĞRENCİ NUMARASI]

### Dersi Veren Öğretim Üyesi
[ÖĞRETİM ÜYESİ ADI SOYADI]

### Tarih
[TARİH]

---

## İçindekiler
1. [Proje Açıklaması](#proje-açıklaması)
2. [Frontend Tasarımları](#frontend-tasarımları)
3. [Proje İsterleri Kanıtları](#proje-isterleri-kanıtları)

## Proje Açıklaması

### Amaç
NyxLine, astronomi meraklılarını bir araya getiren ve Türkiye'deki gözlem noktalarını keşfetmelerini sağlayan bir sosyal medya platformudur. Proje, kullanıcıların gözlem deneyimlerini paylaşabilecekleri, diğer gözlemcilerle etkileşime geçebilecekleri ve en iyi gözlem noktalarını bulabilecekleri kapsamlı bir platform sunmayı amaçlamaktadır.

### Özellikler

#### 1. Kullanıcı Yönetimi
- Kayıt ve giriş sistemi
- Profil yönetimi ve özelleştirme
- Şifre değiştirme ve sıfırlama mekanizmaları
- Kullanıcı rolleri (normal kullanıcı, admin)

#### 2. Sosyal Medya İşlevleri
- Gönderi oluşturma ve paylaşma
- Beğeni ve yorum sistemi
- Kullanıcı takip mekanizması
- Gönderi repost özelliği
- Arama ve keşfet fonksiyonları

#### 3. Gözlem Noktaları Sistemi
- Türkiye'nin en iyi gözlem noktaları veritabanı
- Konum bazlı yakın nokta önerileri
- Detaylı nokta bilgileri (rakım, ışık kirliliği, ulaşım)
- Ekipman önerileri ve kullanım kılavuzları

#### 4. Admin Paneli
- Kullanıcı yönetimi ve moderasyon
- İçerik denetimi ve yönetimi
- Sistem istatistikleri ve raporlama

## Frontend Tasarımları

### 1. Giriş Sayfası (LoginPage.xaml)
![Login Page Screenshot]

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="NyxLine.MAUI.Views.LoginPage">
    <!-- Tasarım detayları -->
</ContentPage>
```

**Sayfa Amacı:** Kullanıcı kimlik doğrulama işlemlerinin gerçekleştirilmesi.

**API İstekleri:**
- Endpoint: `/api/auth/login`
- Method: POST
- Parameters: `UserLoginDto { Email, Password }`
- Response: `ApiResponse<string> { Token }`

### 2. Ana Sayfa (FeedPage.xaml)
![Feed Page Screenshot]

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="NyxLine.MAUI.Views.FeedPage">
    <CollectionView ItemsSource="{Binding Posts}">
        <!-- Post template -->
    </CollectionView>
</ContentPage>
```

**Sayfa Amacı:** Kullanıcı gönderilerinin listelenmesi ve etkileşim.

**API İstekleri:**
- Endpoint: `/api/posts`
- Method: GET
- Parameters: `page, pageSize`
- Response: `List<PostDto>`

## Proje İsterleri Kanıtları

### 1. MAUI Projesi
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Quicksand-Regular.ttf", "QuickRegular");
                fonts.AddFont("Quicksand-Medium.ttf", "QuickMedium");
                fonts.AddFont("Quicksand-SemiBold.ttf", "QuickSemiBold");
                fonts.AddFont("Quicksand-Bold.ttf", "QuickBold");
            });

        // Service registrations
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();

        return builder.Build();
    }
}
```

### 2. Minimal API
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/posts", async (IPostService postService) =>
{
    var posts = await postService.GetPostsAsync();
    return Results.Ok(posts);
});

app.MapPost("/api/posts", async (PostDto post, IPostService postService) =>
{
    var result = await postService.CreatePostAsync(post);
    return Results.Created($"/api/posts/{result.Id}", result);
});
```

### 3. Entity Framework Code First
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Like> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relationship configurations
        modelBuilder.Entity<Follow>()
            .HasKey(f => new { f.FollowerId, f.FollowingId });

        modelBuilder.Entity<Like>()
            .HasKey(l => new { l.UserId, l.PostId });
    }
}
```

### 4. Migration Örnekleri
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// 20250701181742_AddRepostFeature.cs
public partial class AddRepostFeature : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsRepost",
            table: "Posts",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<int>(
            name: "OriginalPostId",
            table: "Posts",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsRepost",
            table: "Posts");

        migrationBuilder.DropColumn(
            name: "OriginalPostId",
            table: "Posts");
    }
}
```

### 5. Veritabanı İşlemleri

#### Listeleme
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// PostService.cs
public async Task<List<PostDto>> GetPostsAsync(int page = 1, int pageSize = 10)
{
    return await _context.Posts
        .Include(p => p.User)
        .Include(p => p.Likes)
        .OrderByDescending(p => p.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new PostDto
        {
            Id = p.Id,
            Content = p.Content,
            UserFullName = p.User.FullName,
            LikeCount = p.Likes.Count
        })
        .ToListAsync();
}
```

#### Ekleme
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// AuthService.cs
public async Task<User> RegisterAsync(UserRegistrationDto model)
{
    var user = new User
    {
        Email = model.Email,
        FullName = model.FullName,
        PasswordHash = HashPassword(model.Password),
        CreatedAt = DateTime.UtcNow
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return user;
}
```

#### Güncelleme
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// UserService.cs
public async Task<bool> UpdateProfileAsync(UpdateProfileDto model)
{
    var user = await _context.Users.FindAsync(model.UserId);
    if (user == null) return false;

    user.FullName = model.FullName;
    user.Bio = model.Bio;
    user.UpdatedAt = DateTime.UtcNow;
    user.UpdatedBy = model.UserId;

    await _context.SaveChangesAsync();
    return true;
}
```

#### Silme
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// PostService.cs
public async Task<bool> DeletePostAsync(int postId, int userId)
{
    var post = await _context.Posts
        .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
    
    if (post == null) return false;

    _context.Posts.Remove(post);
    await _context.SaveChangesAsync();
    return true;
}
```

### 6. MAUI Kontrolleri

#### CollectionView
✓ Gerçekleştirildi

**Kanıt:**
```xaml
<!-- FeedPage.xaml -->
<CollectionView ItemsSource="{Binding Posts}"
                RemainingItemsThreshold="2"
                RemainingItemsThresholdReachedCommand="{Binding LoadMoreCommand}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Frame BorderColor="LightGray"
                   CornerRadius="15"
                   Margin="10">
                <!-- Post template -->
            </Frame>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

#### Picker
✓ Gerçekleştirildi

**Kanıt:**
```xaml
<!-- CreatePostPage.xaml -->
<Picker Title="Gönderi Türü"
        ItemsSource="{Binding PostTypes}"
        SelectedItem="{Binding SelectedPostType}">
</Picker>
```

#### DatePicker
✓ Gerçekleştirildi

**Kanıt:**
```xaml
<!-- RegisterPage.xaml -->
<DatePicker Date="{Binding BirthDate}"
            Format="dd/MM/yyyy"
            MinimumDate="1900-01-01">
</DatePicker>
```

### 7. Service Pattern

#### Interface
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// IPostService.cs
public interface IPostService
{
    Task<List<PostDto>> GetPostsAsync(int page = 1, int pageSize = 10);
    Task<PostDto> CreatePostAsync(CreatePostRequest request);
    Task<bool> UpdatePostAsync(UpdatePostRequest request);
    Task<bool> DeletePostAsync(int postId, int userId);
}
```

#### Implementation
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// PostService.cs
public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public PostService(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    // Interface method implementations
}
```

### 8. OOP Prensipleri
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// Inheritance
public class BaseViewModel : BindableObject
{
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }
}

// Encapsulation & Polymorphism
public class FeedViewModel : BaseViewModel
{
    private ObservableCollection<Post> _posts;
    public ObservableCollection<Post> Posts
    {
        get => _posts;
        set
        {
            _posts = value;
            OnPropertyChanged();
        }
    }
}
```

### 9. Veri Doğrulama
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// UserRegistrationDto.cs
public class UserRegistrationDto
{
    [Required(ErrorMessage = "Email adresi zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [MaxLength(50, ErrorMessage = "Ad Soyad 50 karakterden uzun olamaz")]
    public string FullName { get; set; }
}
```

### 10. LINQ Kullanımı
✓ Gerçekleştirildi

**Kanıt:**
```csharp
// UserService.cs
public async Task<List<UserProfileDto>> SearchUsersAsync(string searchTerm)
{
    return await _context.Users
        .Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm))
        .Select(u => new UserProfileDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            FollowerCount = u.Followers.Count(),
            PostCount = u.Posts.Count()
        })
        .OrderByDescending(u => u.FollowerCount)
        .Take(10)
        .ToListAsync();
}
```

## Sonuç

NyxLine projesi, modern web teknolojilerini kullanarak astronomi tutkunları için kapsamlı bir sosyal medya platformu sunmaktadır. Proje, tüm temel isterleri başarıyla karşılamakta ve kullanıcı deneyimini ön planda tutmaktadır. MAUI framework'ünün sunduğu çoklu platform desteği sayesinde, uygulama farklı işletim sistemlerinde sorunsuz çalışabilmektedir.

Projenin gelecek versiyonlarında:
- Canlı gökyüzü haritası
- Gözlem etkinlikleri oluşturma
- Astrofotoğrafçılık rehberleri
- Teleskop paylaşım platformu

gibi özelliklerin eklenmesi planlanmaktadır. 