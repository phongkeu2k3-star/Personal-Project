using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.DTOs
{
    // Class AssetDto: Dùng để chuyển dữ liệu Asset ra ngoài (Response)
    // Chúng ta không dùng trực tiếp Entity để tránh lộ các thông tin nhạy cảm hoặc cấu trúc DB
    public class AssetDto
    {
        // ID của tài sản
        public int Id { get; set; }

        // Mã giao dịch (Symbol) ví dụ: BTC
        public string Symbol { get; set; } = string.Empty;

        // Tên tài sản ví dụ: Bitcoin
        public string Name { get; set; } = string.Empty;

        // Giá hiện tại
        public decimal CurrentPrice { get; set; }

        // Thời gian cập nhật cuối cùng
        public DateTime LastUpdated { get; set; }
    }
}