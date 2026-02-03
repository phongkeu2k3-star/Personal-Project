CHẠY CODE
Lấy code về sau đó mở source bằng vs 2022
Kiểm tra file Dependencies của từng folder xem nó có các packages chưa ( nếu chưa chuột phải vào Solution -> chọn Restore NuGet Packages)
Mở file appsettings.json trong dự án Finance.WebAPI tại dòng ("DefaultConnection": "Server=localhost; Database=Base-103-Dev; User Id=sa; Password=Ntp080203; TrustServerCertificate=True")->("DefaultConnection": "Server=[tên sever];Database=FinanceRealtimeDb;Trusted_Connection=True;")
Sau đó vào tool trên thanh công cụ chọn NuGet Package Manager -> package manager console
->> ở phần Default project chọn Finance.Infrastructure sau đó gõ lệnh Update-Database
Có lỗi j báo tôi nha khoogn thì hỏi chat



# 📈 Finance Real-time Dashboard

Hệ thống theo dõi và cập nhật giá tài sản tài chính (Crypto, Vàng, Chứng khoán) theo thời gian thực (Real-time) sử dụng công nghệ SignalR và kiến trúc Clean Architecture.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Status](https://img.shields.io/badge/status-In%20Development-green.svg)

---

## 🚀 Giới thiệu (Overview)
Dự án này được xây dựng để giải quyết bài toán hiển thị dữ liệu biến động nhanh mà không cần tải lại trang (F5). Hệ thống bao gồm:
- **Backend:** ASP.NET Core Web API xử lý logic và quản lý kết nối.
- **Worker Service:** Tự động lấy giá từ Binance API cứ mỗi 5 giây.
- **Real-time Engine:** Sử dụng SignalR để đẩy giá (Push notification) xuống trình duyệt.
- **Database:** SQL Server lưu trữ danh mục tài sản và lịch sử giá.

---

## 🏗 Kiến trúc Hệ thống (Clean Architecture)
Dự án tuân thủ nghiêm ngặt mô hình Clean Architecture với 4 tầng độc lập:

| Tên Project | Loại (Type) | Nhiệm vụ chính |
| :--- | :--- | :--- |
| **Finance.Domain** | Class Library | **Tầng lõi (Core):** Chứa Entities (`Asset`), Interfaces. Không phụ thuộc vào bất kỳ tầng nào khác. |
| **Finance.Application** | Class Library | **Tầng nghiệp vụ:** Chứa Logic xử lý, Services, DTOs. Phụ thuộc `Domain`. |
| **Finance.Infrastructure** | Class Library | **Tầng hạ tầng:** Chứa DbContext (EF Core), Migrations, Repository. Phụ thuộc `Application`, `Domain`. |
| **Finance.WebAPI** | Web API | **Tầng giao diện:** Chứa Controllers, SignalR Hubs, Worker Services. Là nơi chạy ứng dụng. |

---

## 🛠 Yêu cầu cài đặt (Prerequisites)
Để chạy được dự án, máy tính của bạn cần cài đặt:
1. **Visual Studio 2022** (Khuyên dùng bản mới nhất).
2. **.NET 8.0 SDK**.
3. **SQL Server** (Bản Express hoặc Developer) hoặc dùng **LocalDB** có sẵn trong Visual Studio.

---
