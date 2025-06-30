# NyxLine

NyxLine, .NET Core Web API ve .NET MAUI kullanılarak geliştirilmiş bir sosyal medya uygulamasıdır.

## Özellikler

- Kullanıcı kaydı ve girişi
- Profil yönetimi
- Gönderi paylaşımı
- Kullanıcı takip sistemi
- Şifre sıfırlama

## Teknolojiler

### Backend (NyxLine.API)
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication

### Mobile (NyxLine.MAUI)
- .NET MAUI
- MVVM Pattern
- HTTP Client

## Kurulum

1. API Projesini Çalıştırma:
```bash
cd NyxLine.API
dotnet run
```

2. MAUI Uygulamasını Çalıştırma:
```bash
cd NyxLine.MAUI
dotnet build -t:Run -f net7.0-android
# veya
dotnet build -t:Run -f net7.0-ios
# veya
dotnet build -t:Run -f net7.0-windows10.0.19041.0
```

## API Endpoints

### Kimlik Doğrulama
- POST /api/auth/register - Yeni kullanıcı kaydı
- POST /api/auth/login - Kullanıcı girişi
- POST /api/auth/forgot-password - Şifre sıfırlama talebi
- POST /api/auth/reset-password - Şifre sıfırlama

### Kullanıcı İşlemleri
- GET /api/users/profile - Kullanıcı profili görüntüleme
- PUT /api/users/profile - Profil güncelleme
- PUT /api/users/change-password - Şifre değiştirme

### Gönderiler
- POST /api/posts - Yeni gönderi oluşturma
- GET /api/posts - Gönderileri listeleme
- PUT /api/posts/{id} - Gönderi güncelleme
- DELETE /api/posts/{id} - Gönderi silme

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. 