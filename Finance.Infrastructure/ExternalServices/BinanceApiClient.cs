using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finance.Infrastructure.ExternalServices
{
    // Class này sẽ chịu trách nhiệm gọi API của Binance
    public class BinanceApiClient
    {
        private readonly HttpClient _httpClient;

        // HttpClient được inject từ Factory (sẽ cấu hình ở Program.cs sau)
        public BinanceApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Hàm giả lập lấy giá (sẽ hoàn thiện logic parse JSON ở giai đoạn sau)
        public async Task<string> GetPriceAsync(string symbol)
        {
            // Gọi API Binance public
            // Ví dụ: https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT
            var response = await _httpClient.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol={symbol}USDT");

            // Đảm bảo request thành công
            response.EnsureSuccessStatusCode();

            // Đọc nội dung trả về
            return await response.Content.ReadAsStringAsync();
        }
    }
}