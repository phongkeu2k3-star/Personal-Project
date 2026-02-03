using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Entities
{
    // Class Asset đại diện cho một tài sản tài chính (Crypto, Stock,...)
    public class Asset
    {
        // Khóa chính (Primary Key) định danh duy nhất cho mỗi tài sản
        public int Id { get; set; }

        // Mã giao dịch của tài sản (ví dụ: "BTC", "ETH"). Chuỗi string không null.
        public string Symbol { get; set; } = string.Empty;

        // Tên đầy đủ của tài sản (ví dụ: "Bitcoin", "Ethereum").
        public string Name { get; set; } = string.Empty;

        // Giá hiện tại của tài sản. Sử dụng kiểu decimal cho độ chính xác cao trong tài chính.
        public decimal CurrentPrice { get; set; }

        // Thời điểm cập nhật giá lần cuối cùng.
        public DateTime LastUpdated { get; set; }

        // Mối quan hệ 1-n: Một Asset có thể có nhiều lịch sử giá (PriceHistory).
        // ICollection cho phép Entity Framework quản lý danh sách này.
        public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
    }
}