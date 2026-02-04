using Finance.Application.Mappings; // Import cấu hình AutoMapper
using Finance.Application.Services; // Import các Service
using Finance.Domain.Interfaces;    // Import các Interface
using Finance.Infrastructure.Data;  // Import DbContext
using Finance.Infrastructure.Repositories; // Import Repository implementation
using Microsoft.EntityFrameworkCore; // Import EF Core

var builder = WebApplication.CreateBuilder(args);

// ==============================================================
// 1. ĐĂNG KÝ SERVICES (Dependency Injection)
// ==============================================================

// Cấu hình kết nối Database lấy từ appsettings.json
// "DefaultConnection" phải khớp với tên trong file appsettings.json bạn đã sửa ở bước trước
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký AutoMapper
// Nó sẽ tự động quét và tìm file MappingProfile trong project Finance.Application
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Đăng ký Repositories (Layer Infrastructure)
// Khi ai đó cần IAssetRepository, hãy đưa cho họ AssetRepository
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();

// Đăng ký Services (Layer Application)
// Khi Controller cần IAssetService, hãy đưa cho họ AssetService
builder.Services.AddScoped<IAssetService, AssetService>();

// Đăng ký HttpClient để gọi API bên ngoài (cho BinanceApiClient sau này)
builder.Services.AddHttpClient();

// Đăng ký Controllers (để tạo REST API)
builder.Services.AddControllers();

// Đăng ký Swagger (tài liệu API tự động) - Rất tiện để test
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký AutoMapper, nó sẽ tự quét các Profile trong Assembly của bạn
builder.Services.AddAutoMapper(typeof(Program));

// Đăng ký SignalR Service
builder.Services.AddSignalR();

// Đăng ký Worker Service (Chạy ngầm)
builder.Services.AddHostedService<Finance.WebAPI.Workers.PriceUpdateWorker>();



// Cấu hình CORS (Cross-Origin Resource Sharing)
// Cho phép Frontend (ở port khác) có thể gọi vào API này
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Cho phép mọi nguồn (trong thực tế nên giới hạn)
              .AllowAnyMethod() // Cho phép mọi method (GET, POST...)
              .AllowAnyHeader(); // Cho phép mọi header
    });
});

var app = builder.Build();

// ==============================================================
// 2. CẤU HÌNH HTTP REQUEST PIPELINE
// ==============================================================

// Nếu đang chạy ở môi trường Development (máy local), bật Swagger lên để test
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Bật HTTPS Redirection (chuyển hướng sang giao thức bảo mật)
app.UseHttpsRedirection();

// ---------------- THÊM MỚI ----------------
app.UseDefaultFiles(); // Cho phép chạy file index.html mặc định
app.UseStaticFiles();  // Cho phép truy cập thư mục wwwroot (css, js)
// ------------------------------------------

// Kích hoạt CORS đã cấu hình ở trên
app.UseCors("AllowAll");

// Map các Controllers vào đường dẫn URL
app.MapControllers();

// Map đường dẫn cho SignalR Hub
// Client sẽ kết nối vào: http://localhost:xxxx/priceHub
app.MapHub<Finance.WebAPI.Hubs.PriceHub>("/priceHub");

// Chạy ứng dụng
app.Run();