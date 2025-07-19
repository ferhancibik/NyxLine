# NyxLine API Dokümantasyonu

## 🔑 Kimlik Doğrulama Endpoints

### Kullanıcı Kaydı
```http
POST /api/auth/register
```

**Request Body:**
```json
{
  "userName": "string",
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string",
  "bio": "string",
  "isGhost": boolean
}
```

**Response:**
```json
{
  "success": true,
  "message": "Kullanıcı başarıyla oluşturuldu",
  "user": {
    "id": "string",
    "userName": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string"
  }
}
```

### Kullanıcı Girişi
```http
POST /api/auth/login
```

**Request Body:**
```json
{
  "emailOrUserName": "string",
  "password": "string"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Giriş başarılı",
  "token": "string",
  "user": {
    "id": "string",
    "userName": "string",
    "email": "string"
  }
}
```

## 👤 Kullanıcı İşlemleri

### Profil Görüntüleme
```http
GET /api/users/profile
```

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
{
  "id": "string",
  "userName": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "bio": "string",
  "profileImagePath": "string",
  "postsCount": 0,
  "followersCount": 0,
  "followingCount": 0
}
```

### Profil Güncelleme
```http
PUT /api/users/profile
```

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "firstName": "string",
  "lastName": "string",
  "bio": "string",
  "profileImageBase64": "string"
}
```

## 📝 Gönderi İşlemleri

### Gönderi Oluşturma
```http
POST /api/posts
```

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "content": "string",
  "imageBase64": "string",
  "type": "Regular"
}
```

### Gönderileri Listeleme
```http
GET /api/posts/feed
```

**Headers:**
```
Authorization: Bearer {token}
```

**Query Parameters:**
```
page: number (default: 1)
pageSize: number (default: 10)
```

## 👥 Takip İşlemleri

### Kullanıcı Takip Etme
```http
POST /api/users/{userId}/follow
```

**Headers:**
```
Authorization: Bearer {token}
```

### Kullanıcı Takibi Bırakma
```http
DELETE /api/users/{userId}/follow
```

**Headers:**
```
Authorization: Bearer {token}
```

## 👨‍💼 Admin İşlemleri

### Haber Oluşturma
```http
POST /api/admin/news
```

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "content": "string",
  "newsTitle": "string",
  "imageBase64": "string",
  "type": "News"
}
```

### Kullanıcı Yönetimi
```http
POST /api/admin/users/{userId}/ban
DELETE /api/admin/users/{userId}/ban
POST /api/admin/users/{userId}/make-admin
DELETE /api/admin/users/{userId}/remove-admin
```

## 🔒 Güvenlik

- Tüm API istekleri HTTPS üzerinden yapılmalıdır
- JWT token'ları 7 gün geçerlidir
- Rate limiting uygulanmaktadır
- Hassas veriler şifrelenerek saklanmaktadır

## 📝 Hata Kodları

| Kod | Açıklama |
|-----|-----------|
| 200 | Başarılı |
| 400 | Geçersiz istek |
| 401 | Yetkisiz erişim |
| 403 | Erişim engellendi |
| 404 | Bulunamadı |
| 500 | Sunucu hatası |

## 📦 Modeller

### User
```json
{
  "id": "string",
  "userName": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "bio": "string",
  "profileImagePath": "string",
  "isGhost": boolean,
  "isAdmin": boolean,
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Post
```json
{
  "id": number,
  "content": "string",
  "imagePath": "string",
  "userId": "string",
  "userFullName": "string",
  "userProfileImage": "string",
  "likeCount": number,
  "isLikedByCurrentUser": boolean,
  "type": "Regular | News",
  "newsTitle": "string",
  "createdAt": "datetime"
}
```

## 🔄 Webhook'lar

Yakında eklenecek... 