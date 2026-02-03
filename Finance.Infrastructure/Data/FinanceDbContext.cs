using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities; // Import các Entity
using Microsoft.EntityFrameworkCore; // Import EF Core
using System.Reflection; // Để quét các file configuration

// Namespace Infrastructure Data
namespace Finance.Infrastructure.Data
{
    // Class FinanceDbContext kế thừa từ DbContext của EF Core
    public class FinanceDbContext : DbContext
    {
        // Constructor nhận vào DbContextOptions (chứa connection string,...)
        // và truyền nó cho lớp cha (base)
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng trong database
        public DbSet<Asset> Assets { get; set; } // Bảng Assets
        public DbSet<PriceHistory> PriceHistories { get; set; } // Bảng PriceHistories

        // Hàm này chạy khi EF Core đang xây dựng model (trước khi tạo DB)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Gọi logic của lớp cha
            base.OnModelCreating(modelBuilder);

            // Tự động quét và áp dụng tất cả các file Configuration trong Assembly hiện tại
            // (giúp code gọn gàng, không phải gọi từng file config thủ công)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}