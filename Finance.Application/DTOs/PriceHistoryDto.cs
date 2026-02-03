using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace cho DTOs
namespace Finance.Application.DTOs
{
    // Class PriceHistoryDto: Dùng để trả về dữ liệu lịch sử giá cho biểu đồ
    public class PriceHistoryDto
    {
        // Giá tại thời điểm đó
        public decimal Price { get; set; }

        // Thời gian ghi nhận giá
        public DateTime Timestamp { get; set; }
    }
}