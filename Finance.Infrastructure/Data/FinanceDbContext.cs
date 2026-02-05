using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities;
using Microsoft.AspNetCore.Identity; // Import thư viện Identity
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Import Identity cho EF Core
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Finance.Infrastructure.Data
{
    // QUAN TRỌNG: Đổi kế thừa từ DbContext sang IdentityDbContext<IdentityUser>
    // IdentityUser là class mặc định chứa thông tin User (Id, UserName, Email, PassHash...)
    public class FinanceDbContext : IdentityDbContext<IdentityUser>
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // BẮT BUỘC: Gọi hàm base để Identity tạo các bảng của nó
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}