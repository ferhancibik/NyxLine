using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NyxLine.API.Data;
using NyxLine.API.DTOs;
using NyxLine.API.Models;
using NyxLine.API.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;        // Rakam zorunlu değil
    options.Password.RequireLowercase = false;    // Küçük harf zorunlu değil
    options.Password.RequireNonAlphanumeric = false; // Özel karakter zorunlu değil
    options.Password.RequireUppercase = false;    // Büyük harf zorunlu değil
    options.Password.RequiredLength = 3;          // En az 3 karakter
    options.User.RequireUniqueEmail = true;       // E-posta benzersiz olmalı
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IFileService, FileService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NyxLine API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Helper method to get current user ID
string? GetCurrentUserId(ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

// AUTH ENDPOINTS
app.MapPost("/api/auth/register", async (UserRegistrationDto dto, IAuthService authService) =>
{
    var result = await authService.RegisterAsync(dto);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message, user = new { result.User?.Id, result.User?.UserName, result.User?.Email } });
    }
    return Results.BadRequest(new { message = result.Message });
}).WithTags("Auth");

app.MapPost("/api/auth/login", async (UserLoginDto dto, IAuthService authService) =>
{
    var result = await authService.LoginAsync(dto);
    if (result.Success)
    {
        return Results.Ok(new { 
            message = result.Message, 
            token = result.Token,
            user = new { 
                result.User?.Id, 
                result.User?.UserName, 
                result.User?.Email,
                result.User?.FirstName,
                result.User?.LastName,
                result.User?.IsGhost
            }
        });
    }
    return Results.BadRequest(new { message = result.Message });
}).WithTags("Auth");

app.MapPost("/api/auth/change-password", async (ChangePasswordDto dto, IAuthService authService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    var result = await authService.ChangePasswordAsync(userId, dto);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Auth");

app.MapPost("/api/auth/logout", async (IAuthService authService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    await authService.LogoutAsync(userId);
    return Results.Ok(new { message = "Çıkış başarılı" });
}).RequireAuthorization().WithTags("Auth");

app.MapPost("/api/auth/forgot-password", async (ForgotPasswordDto dto, IAuthService authService) =>
{
    var result = await authService.ForgotPasswordAsync(dto);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message, token = result.Token });
    }
    return Results.BadRequest(new { message = result.Message });
}).WithTags("Auth");

app.MapPost("/api/auth/reset-password", async (ResetPasswordDto dto, IAuthService authService) =>
{
    var result = await authService.ResetPasswordAsync(dto);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).WithTags("Auth");

// USER ENDPOINTS
app.MapGet("/api/users/profile", async (IUserService userService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    var profile = await userService.GetUserProfileAsync(userId, userId);
    if (profile == null) return Results.NotFound();
    
    return Results.Ok(profile);
}).RequireAuthorization().WithTags("Users");

app.MapGet("/api/users/{username}", async (string username, IUserService userService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    var profile = await userService.GetUserByUserNameAsync(username, currentUserId);
    if (profile == null) return Results.NotFound();
    
    return Results.Ok(profile);
}).WithTags("Users");

app.MapPut("/api/users/profile", async (UpdateProfileDto dto, IUserService userService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    var result = await userService.UpdateProfileAsync(userId, dto);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Users");

app.MapGet("/api/users/search", async (string q, IUserService userService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    var isGhost = user.FindFirst("IsGhost")?.Value == "True";
    
    if (isGhost)
    {
        return Results.Forbid();
    }
    
    var users = await userService.SearchUsersAsync(q, currentUserId);
    return Results.Ok(users);
}).RequireAuthorization().WithTags("Users");

app.MapPost("/api/users/{userId}/follow", async (string userId, IUserService userService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    if (currentUserId == null) return Results.Unauthorized();
    
    var result = await userService.FollowUserAsync(currentUserId, userId);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Users");

app.MapDelete("/api/users/{userId}/follow", async (string userId, IUserService userService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    if (currentUserId == null) return Results.Unauthorized();
    
    var result = await userService.UnfollowUserAsync(currentUserId, userId);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Users");

app.MapGet("/api/users/{userId}/followers", async (string userId, IUserService userService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    var followers = await userService.GetFollowersAsync(userId, currentUserId);
    return Results.Ok(followers);
}).WithTags("Users");

app.MapGet("/api/users/{userId}/following", async (string userId, IUserService userService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    var following = await userService.GetFollowingAsync(userId, currentUserId);
    return Results.Ok(following);
}).WithTags("Users");

// POST ENDPOINTS
app.MapPost("/api/posts", async (CreatePostDto dto, IPostService postService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    var result = await postService.CreatePostAsync(userId, dto);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message, post = result.Post });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Posts");

app.MapPut("/api/posts/{postId:int}", async (int postId, UpdatePostDto dto, IPostService postService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    var result = await postService.UpdatePostAsync(postId, userId, dto);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Posts");

app.MapDelete("/api/posts/{postId:int}", async (int postId, IPostService postService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    var result = await postService.DeletePostAsync(postId, userId);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Posts");

app.MapGet("/api/posts/{postId:int}", async (int postId, IPostService postService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    var post = await postService.GetPostByIdAsync(postId, currentUserId);
    if (post == null) return Results.NotFound();
    
    return Results.Ok(post);
}).WithTags("Posts");

app.MapGet("/api/posts", async (int page, int pageSize, IPostService postService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    var posts = await postService.GetAllPostsAsync(currentUserId, page, pageSize);
    return Results.Ok(posts);
}).WithTags("Posts");

app.MapGet("/api/users/{userId}/posts", async (string userId, int page, int pageSize, IPostService postService, ClaimsPrincipal user) =>
{
    var currentUserId = GetCurrentUserId(user);
    var posts = await postService.GetUserPostsAsync(userId, currentUserId, page, pageSize);
    return Results.Ok(posts);
}).WithTags("Posts");

app.MapGet("/api/posts/feed", async (int page, int pageSize, IPostService postService) =>
{
    // Geçici: Tamamen public endpoint, herkes tüm gönderileri görebilir
    var posts = await postService.GetAllPostsAsync(null, page, pageSize);
    return Results.Ok(posts);
}).WithTags("Posts");

app.MapPost("/api/posts/{postId:int}/like", async (int postId, IPostService postService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    var result = await postService.LikePostAsync(postId, userId);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Posts");

app.MapDelete("/api/posts/{postId:int}/like", async (int postId, IPostService postService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    var result = await postService.UnlikePostAsync(postId, userId);
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Posts");

// TEST ENDPOINT - Profil fotoğrafı test için
app.MapPost("/api/test/profile-photo", async (IUserService userService, ClaimsPrincipal user) =>
{
    var userId = GetCurrentUserId(user);
    if (userId == null) return Results.Unauthorized();
    
    Console.WriteLine($"[TEST] Test profil fotoğrafı endpoint'i çağrıldı. UserId: {userId}");
    
    // Test Base64 image (1x1 pixel kırmızı PNG)
    var testBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChAFAcCfzSgAAAABJRU5ErkJggg==";
    
    var updateDto = new UpdateProfileDto
    {
        FirstName = "Test",
        LastName = "User",
        Bio = "Test bio",
        ProfileImageBase64 = testBase64
    };
    
    var result = await userService.UpdateProfileAsync(userId, updateDto);
    Console.WriteLine($"[TEST] Test sonucu: Success={result.Success}, Message={result.Message}");
    
    if (result.Success)
    {
        return Results.Ok(new { message = result.Message, testImage = "1x1 kırmızı pixel yüklendi" });
    }
    return Results.BadRequest(new { message = result.Message });
}).RequireAuthorization().WithTags("Test");

app.Run();
