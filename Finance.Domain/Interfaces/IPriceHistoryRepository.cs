using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities;

// Namespace Interface
namespace Finance.Domain.Interfaces
{
    // Interface định nghĩa các thao tác với bảng lịch sử giá
    public interface IPriceHistoryRepository
    {
        // Thêm một bản ghi lịch sử giá mới vào database.
        Task AddPriceHistoryAsync(PriceHistory priceHistory);

        // Lấy danh sách lịch sử giá của một tài sản cụ thể (dựa theo AssetId).
        // Dùng để vẽ biểu đồ cho tài sản đó.
        Task<IEnumerable<PriceHistory>> GetHistoryByAssetIdAsync(int assetId);
    }
}