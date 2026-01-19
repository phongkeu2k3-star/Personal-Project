using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Finance.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Khai báo bảng Assets trong SQL Server
        public DbSet<Asset> Assets { get; set; }

        //Hàm tạo thử một entity để hiển thị
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Finance.Domain.Entities.Asset>().HasData(
                new Finance.Domain.Entities.Asset { Id = 1, Symbol = "BTC", Name = "Bitcoin", CurrentPrice = 65000, LastUpdated = DateTime.Now },
                new Finance.Domain.Entities.Asset { Id = 2, Symbol = "ETH", Name = "Ethereum", CurrentPrice = 3500, LastUpdated = DateTime.Now }
            );
        }
    }


}

