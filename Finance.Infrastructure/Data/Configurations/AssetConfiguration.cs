using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities; // Import Entity Asset
using Microsoft.EntityFrameworkCore; // Import EF Core
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Import Builder

namespace Finance.Infrastructure.Data.Configurations
{
    // Class cấu hình riêng cho bảng Asset
    public class AssetConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            // Đặt khóa chính là Id
            builder.HasKey(a => a.Id);

            // Cấu hình cột Symbol: Bắt buộc có (IsRequired), độ dài tối đa 10 ký tự
            builder.Property(a => a.Symbol)
                .IsRequired()
                .HasMaxLength(10);

            // Cấu hình cột Name: Bắt buộc có, độ dài tối đa 100
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Cấu hình cột CurrentPrice: Kiểu decimal, độ chính xác (18, 8) 
            // (18 tổng chữ số, 8 số sau dấu phẩy - phù hợp cho Crypto)
            builder.Property(a => a.CurrentPrice)
                .HasColumnType("decimal(18,8)");

            // Cấu hình mối quan hệ 1-N: Một Asset có nhiều PriceHistories
            builder.HasMany(a => a.PriceHistories)
                .WithOne(ph => ph.Asset) // Mỗi PriceHistory thuộc về 1 Asset
                .HasForeignKey(ph => ph.AssetId) // Khóa ngoại là AssetId
                .OnDelete(DeleteBehavior.Cascade); // Nếu xóa Asset, xóa luôn lịch sử giá
        }
    }
}