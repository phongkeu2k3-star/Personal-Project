using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities;

// Namespace chứa các Interface của Domain
namespace Finance.Domain.Interfaces
{
    // Interface định nghĩa các phương thức thao tác với dữ liệu Asset
    public interface IAssetRepository
    {
        // Lấy danh sách tất cả các tài sản có trong database.
        // Trả về một Task chứa danh sách Asset (IEnumerable).
        Task<IEnumerable<Asset>> GetAllAssetsAsync();

        // Lấy thông tin chi tiết một tài sản dựa vào ID.
        // Trả về Asset hoặc null nếu không tìm thấy.
        Task<Asset?> GetAssetByIdAsync(int id);

        // Lấy thông tin tài sản dựa vào mã Symbol (ví dụ: tìm "BTC").
        Task<Asset?> GetAssetBySymbolAsync(string symbol);

        // Thêm một tài sản mới vào database.
        Task AddAssetAsync(Asset asset);

        // Cập nhật thông tin của một tài sản đã tồn tại (ví dụ: cập nhật giá mới).
        Task UpdateAssetAsync(Asset asset);

        // Kiểm tra xem một Symbol đã tồn tại trong database chưa.
        Task<bool> AssetExistsAsync(string symbol);
    }
}