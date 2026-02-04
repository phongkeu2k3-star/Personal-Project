using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Application.DTOs;

// Namespace Services
namespace Finance.Application.Services
{
    // Interface định nghĩa các chức năng mà Service cung cấp cho Controller (API)
    public interface IAssetService
    {
        // Lấy danh sách tất cả tài sản dưới dạng DTO
        Task<IEnumerable<AssetDto>> GetAllAssetsAsync();

        // Lấy chi tiết một tài sản theo ID
        Task<AssetDto?> GetAssetByIdAsync(int id);

        // Tạo mới một tài sản
        Task<AssetDto> CreateAssetAsync(CreateAssetDto createAssetDto);

        // Lấy lịch sử giá của một tài sản
        Task<IEnumerable<PriceHistoryDto>> GetAssetHistoryAsync(int assetId);

        //hàm xóa
        Task DeleteAssetAsync(int id);
    }
}