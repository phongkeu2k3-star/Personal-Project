using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entities
{
    public class Asset
    {
        public int Id { get; set; } // Khóa chính tự tăng
        public string Symbol { get; set; } = string.Empty; // Ví dụ: BTC, GOLD
        public string Name { get; set; } = string.Empty;   // Ví dụ: Bitcoin
        public decimal CurrentPrice { get; set; }          // Giá hiện tại
        public double PriceChange24h { get; set; }        // Biến động %
        public DateTime LastUpdated { get; set; }          // Thời gian cập nhật cuối
    }
}
