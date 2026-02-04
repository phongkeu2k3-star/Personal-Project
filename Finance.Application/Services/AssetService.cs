using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Finance.Application.DTOs;
using Finance.Domain.Entities;
using Finance.Domain.Interfaces;
using System.Threading.Tasks;

// Namespace Services
namespace Finance.Application.Services
{
    // Class AssetService thực thi interface IAssetService
    public class AssetService : IAssetService
    {
        // Khai báo các biến private readonly để Dependency Injection (DI)
        private readonly IAssetRepository _assetRepository; // Giao tiếp với DB Asset
        private readonly IPriceHistoryRepository _priceHistoryRepository; // Giao tiếp với DB PriceHistory
        private readonly IMapper _mapper; // Công cụ convert object

        // Constructor: Nhận các dependency thông qua DI container
        public AssetService(IAssetRepository assetRepository, IPriceHistoryRepository priceHistoryRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _priceHistoryRepository = priceHistoryRepository;
            _mapper = mapper;
        }

        // Hàm tạo mới tài sản
        public async Task<AssetDto> CreateAssetAsync(CreateAssetDto createAssetDto)
        {
            // Kiểm tra xem mã symbol đã tồn tại chưa (Logic nghiệp vụ)
            var exists = await _assetRepository.AssetExistsAsync(createAssetDto.Symbol);
            if (exists)
            {
                // Nếu tồn tại thì ném lỗi (hoặc xử lý tùy ý)
                throw new InvalidOperationException($"Asset with symbol {createAssetDto.Symbol} already exists.");
            }

            // 1. Chuyển đổi từ CreateAssetDto sang Entity Asset
            var asset = _mapper.Map<Asset>(createAssetDto);

            // Thiết lập giá trị mặc định
            asset.LastUpdated = DateTime.UtcNow;
            asset.CurrentPrice = 0; // Giá ban đầu là 0

            // 2. Lưu vào database thông qua Repository
            await _assetRepository.AddAssetAsync(asset);

            // 3. Chuyển đổi ngược lại Entity vừa lưu sang DTO để trả về cho người dùng
            return _mapper.Map<AssetDto>(asset);
        }

        // Hàm lấy danh sách tất cả tài sản
        public async Task<IEnumerable<AssetDto>> GetAllAssetsAsync()
        {
            // Lấy list entity từ Repository
            var assets = await _assetRepository.GetAllAssetsAsync();

            // Map danh sách Entity sang danh sách DTO
            return _mapper.Map<IEnumerable<AssetDto>>(assets);
        }

        // Hàm lấy chi tiết tài sản theo ID
        public async Task<AssetDto?> GetAssetByIdAsync(int id)
        {
            var asset = await _assetRepository.GetAssetByIdAsync(id);
            // Nếu không tìm thấy (null) thì trả về null, ngược lại map sang DTO
            return asset == null ? null : _mapper.Map<AssetDto>(asset);
        }

        // Hàm lấy lịch sử giá
        public async Task<IEnumerable<PriceHistoryDto>> GetAssetHistoryAsync(int assetId)
        {
            var history = await _priceHistoryRepository.GetHistoryByAssetIdAsync(assetId);
            return _mapper.Map<IEnumerable<PriceHistoryDto>>(history);
        }
        //hàm xóa
        public async Task DeleteAssetAsync(int id)
        {
            // B1: Dùng ID để tìm lấy đối tượng Asset từ DB lên trước
            var assetToDelete = await _assetRepository.GetAssetByIdAsync(id);

            // B2: Kiểm tra nếu tìm thấy thì mới xóa
            if (assetToDelete != null)
            {
                // Lỗi "cannot convert int to Asset" sẽ hết vì ta truyền assetToDelete (kiểu Asset)
                // chứ không truyền id (kiểu int) nữa.
                await _assetRepository.DeleteAssetAsync(assetToDelete);
            }
        }
    }
}