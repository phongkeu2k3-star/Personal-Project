using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Configurations
{
    // Class cấu hình riêng cho bảng PriceHistory
    public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
    {
        public void Configure(EntityTypeBuilder<PriceHistory> builder)
        {
            // Khóa chính
            builder.HasKey(ph => ph.Id);

            // Cấu hình cột Price: Độ chính xác cao (18,8)
            builder.Property(ph => ph.Price)
                .HasColumnType("decimal(18,8)");

            // Tạo Index cho cột AssetId và Timestamp để truy vấn lịch sử nhanh hơn
            builder.HasIndex(ph => new { ph.AssetId, ph.Timestamp });
        }
    }
}