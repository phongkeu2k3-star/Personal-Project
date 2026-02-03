using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace thuộc về Domain Entities
namespace Finance.Domain.Entities
{
    // Class PriceHistory lưu trữ lịch sử giá của một tài sản tại một thời điểm cụ thể
    public class PriceHistory
    {
        // Khóa chính của bảng lịch sử giá
        public int Id { get; set; }

        // Khóa ngoại (Foreign Key) liên kết với bảng Asset.
        // Xác định giá này thuộc về tài sản nào.
        public int AssetId { get; set; }

        // Giá trị của tài sản tại thời điểm lưu trữ.
        public decimal Price { get; set; }

        // Thời gian ghi nhận mức giá này.
        public DateTime Timestamp { get; set; }

        // Navigation Property: Thuộc tính điều hướng giúp truy cập ngược lại thông tin Asset từ lịch sử giá.
        // "virtual" hỗ trợ Lazy Loading trong Entity Framework (nếu cần).
        public virtual Asset? Asset { get; set; }
    }
}