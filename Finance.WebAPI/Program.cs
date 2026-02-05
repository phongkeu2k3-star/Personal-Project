using Finance.Application.Mappings;
using Finance.Application.Services;
using Finance.Domain.Interfaces;
using Finance.Infrastructure.Data;
using Finance.Infrastructure.Repositories;
using Finance.WebAPI.Services; // Import AuthService
using Microsoft.AspNetCore.Authentication.JwtBearer; // Import cấu hình JWT
using Microsoft.AspNetCore.Identity; // Import Identity
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Import Token Validation
using Finance.Domain.Interfaces;
using Finance.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. KẾT NỐI DATABASE
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. BỔ SUNG: Đăng ký Repository
builder.Services.AddScoped<IAssetRepository, AssetRepository>();

// --- 2. CẤU HÌNH IDENTITY (MỚI) ---
// Thêm Identity vào hệ thống, sử dụng EntityFramework để lưu dữ liệu
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<FinanceDbContext>() // Lưu user vào FinanceDbContext
    .AddDefaultTokenProviders(); // Cung cấp chức năng reset pass, confirm email...

// --- 3. CẤU HÌNH JWT AUTHENTICATION (MỚI) ---
// Đăng ký dịch vụ xác thực
builder.Services.AddAuthentication(options =>
{
    // Thiết lập mặc định là dùng JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Cấu hình chi tiết cho JWT
.AddJwtBearer(options =>
{
    options.SaveToken = true; // Lưu token server nhận được
    options.RequireHttpsMetadata = false; // Tắt yêu cầu HTTPS (dev only)
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true, // Kiểm tra xem có đúng Server mình phát hành không
        ValidateAudience = true, // Kiểm tra xem có đúng Client được phép dùng không
        ValidAudience = builder.Configuration["JWT:ValidAudience"], // Lấy từ appsettings
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"], // Lấy từ appsettings
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])) // Kiểm tra chữ ký bằng Key bí mật
    };
});

// 4. ĐĂNG KÝ SERVICES DI
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();

// Đăng ký AuthService (MỚI)
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient();
builder.Services.AddControllers();

// Cấu hình Swagger để hỗ trợ nút "Authorize" (Nhập token)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Định nghĩa cấu hình bảo mật cho Swagger UI
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập token vào ô bên dưới. Ví dụ: Bearer abcxyz123...",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    // Yêu cầu Swagger sử dụng cấu hình bảo mật trên cho các API
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddSignalR();
builder.Services.AddHostedService<Finance.WebAPI.Workers.PriceUpdateWorker>();

var app = builder.Build();

// --- PIPELINE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// --- KÍCH HOẠT AUTHENTICATION (QUAN TRỌNG) ---
// Phải đặt UseAuthentication TRƯỚC UseAuthorization
app.UseAuthentication(); // 1. Xác định "Bạn là ai?" (Check token)
app.UseAuthorization();  // 2. Xác định "Bạn được làm gì?" (Check quyền)

app.MapControllers();
app.MapHub<Finance.WebAPI.Hubs.PriceHub>("/priceHub");

app.Run();