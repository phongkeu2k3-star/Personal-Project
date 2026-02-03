using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities;
using Finance.Domain.Interfaces;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore; // Để dùng .ToListAsync()

namespace Finance.Infrastructure.Repositories
{
    // Class AssetRepository thực thi IAssetRepository
    public class AssetRepository : IAssetRepository
    {
        // Khai báo DbContext
        private readonly FinanceDbContext _context;

        // Inject DbContext vào
        public AssetRepository(FinanceDbContext context)
        {
            _context = context;
        }

        // Thêm tài sản mới
        public async Task AddAssetAsync(Asset asset)
        {
            await _context.Assets.AddAsync(asset); // Thêm vào bộ nhớ
            await _context.SaveChangesAsync(); // Lưu xuống DB thật
        }

        // Kiểm tra Symbol tồn tại
        public async Task<bool> AssetExistsAsync(string symbol)
        {
            // Dùng AnyAsync để kiểm tra nhanh
            return await _context.Assets.AnyAsync(a => a.Symbol == symbol);
        }

        // Lấy tất cả tài sản
        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            return await _context.Assets.ToListAsync();
        }

        // Lấy tài sản theo ID
        public async Task<Asset?> GetAssetByIdAsync(int id)
        {
            return await _context.Assets.FindAsync(id);
        }

        // Lấy tài sản theo Symbol
        public async Task<Asset?> GetAssetBySymbolAsync(string symbol)
        {
            return await _context.Assets.FirstOrDefaultAsync(a => a.Symbol == symbol);
        }

        // Cập nhật tài sản
        public async Task UpdateAssetAsync(Asset asset)
        {
            _context.Assets.Update(asset); // Đánh dấu là đã sửa
            await _context.SaveChangesAsync(); // Lưu thay đổi
        }
    }
}