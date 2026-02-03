using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities;
using Finance.Domain.Interfaces;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories
{
    public class PriceHistoryRepository : IPriceHistoryRepository
    {
        private readonly FinanceDbContext _context;

        public PriceHistoryRepository(FinanceDbContext context)
        {
            _context = context;
        }

        // Thêm lịch sử giá mới
        public async Task AddPriceHistoryAsync(PriceHistory priceHistory)
        {
            await _context.PriceHistories.AddAsync(priceHistory);
            await _context.SaveChangesAsync();
        }

        // Lấy lịch sử giá của một tài sản
        public async Task<IEnumerable<PriceHistory>> GetHistoryByAssetIdAsync(int assetId)
        {
            return await _context.PriceHistories
                .Where(ph => ph.AssetId == assetId) // Lọc theo AssetId
                .OrderByDescending(ph => ph.Timestamp) // Sắp xếp mới nhất lên đầu
                .Take(100) // Chỉ lấy 100 bản ghi gần nhất để tránh quá tải
                .ToListAsync();
        }
    }
}