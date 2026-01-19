using Finance.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Finance.WebAPI.Services
{
    public class PriceWorker : BackgroundService
    {
        private readonly IHubContext<PriceHub> _hubContext;
        private readonly Random _random = new Random();

        public PriceWorker(IHubContext<PriceHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //vòng lặp để mình lấy giá vàng từ một trang web nào đó cai này mình làm sau
            while (!stoppingToken.IsCancellationRequested)
            {
                // Giả lập lấy giá mới (Sau này mình sẽ gọi API thật của Binance ở đây)
                var btcPrice = 65000 + _random.Next(-100, 100);

                // Gửi giá mới tới tất cả mọi người đang xem web
                await _hubContext.Clients.All.SendAsync("ReceivePriceUpdate", "BTC", btcPrice);

                // Nghỉ 5 giây rồi làm tiếp
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}