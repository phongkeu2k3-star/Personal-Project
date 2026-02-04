using Finance.Domain.Entities;
using Finance.Domain.Interfaces;
using Finance.Infrastructure.ExternalServices; // Để gọi Binance API
using Finance.WebAPI.Hubs; // Để gọi SignalR
using Microsoft.AspNetCore.SignalR; // Interface SignalR
using Microsoft.Extensions.Hosting; // Interface BackgroundService
using Microsoft.Extensions.DependencyInjection; // Để tạo Scope
using Microsoft.Extensions.Logging; // Để ghi log
using System;
using System.Text.Json; // Để đọc JSON từ Binance
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Finance.WebAPI.Workers
{
    // Class Worker kế thừa BackgroundService để chạy ngầm
    public class PriceUpdateWorker : BackgroundService
    {
        private readonly ILogger<PriceUpdateWorker> _logger;
        private readonly IHubContext<PriceHub> _hubContext; // Dùng để gửi tin nhắn SignalR
        private readonly IServiceScopeFactory _scopeFactory; // Quan trọng: Dùng để tạo scope mới
        private readonly IHttpClientFactory _httpClientFactory; // Dùng tạo Client gọi API

        public PriceUpdateWorker(
            ILogger<PriceUpdateWorker> logger,
            IHubContext<PriceHub> hubContext,
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
        }

        // Hàm này sẽ chạy vô tận cho đến khi tắt ứng dụng
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Price Update Worker running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1. Tạo Scope mới (Vì Worker là Singleton, còn Repository là Scoped)
                    // Nếu không tạo scope, bạn sẽ gặp lỗi khi gọi Repository
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // Lấy các service cần thiết từ scope
                        var assetRepo = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
                        var historyRepo = scope.ServiceProvider.GetRequiredService<IPriceHistoryRepository>();

                        // Lấy danh sách tất cả tài sản đang theo dõi
                        var assets = await assetRepo.GetAllAssetsAsync();

                        // Tạo HttpClient để gọi Binance
                        var client = _httpClientFactory.CreateClient();
                        // (Tạm thời dùng logic gọi API trực tiếp ở đây để đơn giản hóa việc parse JSON)

                        foreach (var asset in assets)
                        {
                            // 2. Gọi API Binance lấy giá
                            // API Binance trả về dạng: {"symbol":"BTCUSDT","price":"45000.00000000"}
                            var response = await client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol={asset.Symbol}USDT");

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();
                                using var doc = JsonDocument.Parse(jsonString);

                                // Parse giá từ chuỗi string sang decimal
                                if (decimal.TryParse(doc.RootElement.GetProperty("price").GetString(), out decimal currentPrice))
                                {
                                    // 3. Cập nhật vào Database
                                    asset.CurrentPrice = currentPrice;
                                    asset.LastUpdated = DateTime.UtcNow;

                                    // Lưu Asset cập nhật
                                    await assetRepo.UpdateAssetAsync(asset);

                                    // Lưu Lịch sử giá
                                    await historyRepo.AddPriceHistoryAsync(new PriceHistory
                                    {
                                        AssetId = asset.Id,
                                        Price = currentPrice,
                                        Timestamp = DateTime.UtcNow
                                    });

                                    // 4. Bắn tín hiệu qua SignalR
                                    // Gửi sự kiện tên "ReceivePriceUpdate" kèm object dữ liệu
                                    await _hubContext.Clients.All.SendAsync("ReceivePriceUpdate", new
                                    {
                                        Symbol = asset.Symbol,
                                        Price = currentPrice,
                                        Timestamp = DateTime.UtcNow
                                    });

                                    _logger.LogInformation($"Updated {asset.Symbol}: {currentPrice}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching prices");
                }

                // Chờ 5 giây trước khi chạy vòng lặp tiếp theo
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}