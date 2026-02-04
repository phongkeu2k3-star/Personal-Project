
## 📈 Finance Real-time Dashboard

Hệ thống theo dõi và cập nhật giá tài sản tài chính (Crypto, Vàng, Chứng khoán) theo thời gian thực (Real-time) sử dụng công nghệ SignalR và kiến trúc Clean Architecture.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Status](https://img.shields.io/badge/status-In%20Development-green.svg)

---

### 🚀 Giới thiệu (Overview)
Dự án này được xây dựng để giải quyết bài toán hiển thị dữ liệu biến động nhanh mà không cần tải lại trang (F5). Hệ thống bao gồm:
- **Backend:** ASP.NET Core Web API xử lý logic và quản lý kết nối.
- **Worker Service:** Tự động lấy giá từ Binance API cứ mỗi 5 giây.
- **Real-time Engine:** Sử dụng SignalR để đẩy giá (Push notification) xuống trình duyệt.
- **Database:** SQL Server lưu trữ danh mục tài sản và lịch sử giá.

---

### 🏗 Kiến trúc Hệ thống (Clean Architecture)
Dự án tuân thủ nghiêm ngặt mô hình Clean Architecture với 4 tầng độc lập:

| Tên Project | Loại (Type) | Nhiệm vụ chính |
| :--- | :--- | :--- |
| **Finance.Domain** | Class Library | **Tầng lõi (Core):** Chứa Entities (`Asset`), Interfaces. Không phụ thuộc vào bất kỳ tầng nào khác. |
| **Finance.Application** | Class Library | **Tầng nghiệp vụ:** Chứa Logic xử lý, Services, DTOs. Phụ thuộc `Domain`. |
| **Finance.Infrastructure** | Class Library | **Tầng hạ tầng:** Chứa DbContext (EF Core), Migrations, Repository. Phụ thuộc `Application`, `Domain`. |
| **Finance.WebAPI** | Web API | **Tầng giao diện:** Chứa Controllers, SignalR Hubs, Worker Services. Là nơi chạy ứng dụng. |

---

### 🛠 Yêu cầu cài đặt (Prerequisites)
Để chạy được dự án, máy tính của bạn cần cài đặt:
1. **Visual Studio 2022** (Khuyên dùng bản mới nhất).
2. **.NET 8.0 SDK**.
3. **SQL Server** (Bản Express hoặc Developer) hoặc dùng **LocalDB** có sẵn trong Visual Studio.

---

# 📑 TÀI LIỆU ĐẶC TẢ KỸ THUẬT: FINANCE PRO (PHASE 2)

| Project Info | Detail |
| :--- | :--- |
| **Dự án** | Finance Real-time Dashboard & Portfolio Manager |
| **Phiên bản** | 2.0 (Planned) |
| **Mục tiêu** | Chuyển đổi từ Dashboard theo dõi giá chung sang Hệ thống Quản lý Tài sản Số Cá nhân hóa. |
| **Trạng thái** | 🟡 Pending Implementation |

---

## 1. 🎯 MỤC TIÊU & PHẠM VI (SCOPE)

Trong Phase 2, hệ thống sẽ tập trung vào tính năng **User-Centric (Lấy người dùng làm trung tâm)**. Mỗi người dùng sẽ có tài khoản riêng để:
1.  Quản lý danh mục đầu tư (Portfolio) cá nhân.
2.  Theo dõi Lời/Lỗ (Profit & Loss - PnL) thực tế dựa trên lịch sử giao dịch.
3.  Cài đặt cảnh báo giá (Price Alerts) riêng biệt.

---

## 2. 🧩 CÁC MODULE CHỨC NĂNG (FUNCTIONAL REQUIREMENTS)

### 2.1. Module Xác thực (Authentication)
* **Đăng ký / Đăng nhập:** Hỗ trợ Email & Password.
* **Bảo mật API:** Sử dụng **JWT (JSON Web Token)**. Token có thời hạn (ví dụ: 24h) và Refresh Token.
* **Phân quyền:**
    * `Guest`: Chỉ xem giá thị trường.
    * `User`: Có quyền thao tác với Portfolio, Alerts.

### 2.2. Module Quản lý Danh mục (Portfolio Management)
Hệ thống không chỉ lưu user đang giữ coin nào, mà lưu chi tiết lịch sử giao dịch.
* **Transaction Recording:** Người dùng nhập lệnh Mua/Bán (Coin, Số lượng, Giá khớp lệnh, Thời gian).
* **Real-time PnL Calculation:**
    * `Giá trị hiện tại` = Số lượng coin * Giá thị trường (Real-time).
    * `Tổng Lời/Lỗ` = Giá trị hiện tại - Tổng vốn bỏ ra.
* **Asset Allocation:** Biểu đồ tròn (Pie Chart) hiển thị tỷ trọng phân bổ tài sản.

### 2.3. Module Cảnh báo (Price Alerts)
* User cài đặt ngưỡng giá (VD: *Báo tôi khi BTC > $100k*).
* Hệ thống tự động quét giá nền (Background Worker) và gửi thông báo khi khớp điều kiện.
* **Kênh thông báo:** In-app Notification (SignalR Toast) & Email.

---

## 3. 💾 THIẾT KẾ CƠ SỞ DỮ LIỆU (DATABASE SCHEMA)

Sử dụng **SQL Server** với Entity Framework Core. Cấu trúc bảng dự kiến:

### 3.1. Identity Tables (Có sẵn từ ASP.NET Identity)
* `AspNetUsers`: Lưu thông tin đăng nhập, Password Hash.

### 3.2. Business Tables

#### `Portfolios` (Tổng hợp danh mục)
*Lưu trạng thái hiện tại của từng coin mà user đang nắm giữ.*
| Column | Type | Description |
| :--- | :--- | :--- |
| `Id` | INT (PK) | |
| `UserId` | GUID (FK) | Link tới bảng Users |
| `AssetSymbol` | NVARCHAR | VD: BTC, ETH |
| `TotalQuantity`| DECIMAL | Tổng số lượng đang giữ |
| `AvgBuyPrice` | DECIMAL | Giá mua trung bình (để tính PnL) |

#### `Transactions` (Lịch sử giao dịch)
*Lưu chi tiết từng lệnh mua/bán để audit.*
| Column | Type | Description |
| :--- | :--- | :--- |
| `Id` | INT (PK) | |
| `PortfolioId` | INT (FK) | Link tới bảng Portfolios |
| `Type` | ENUM | 0 = Buy, 1 = Sell |
| `Amount` | DECIMAL | Số lượng coin giao dịch |
| `Price` | DECIMAL | Giá tại thời điểm giao dịch |
| `Date` | DATETIME | Thời gian thực hiện |

#### `PriceAlerts` (Cảnh báo)
| Column | Type | Description |
| :--- | :--- | :--- |
| `Id` | INT (PK) | |
| `UserId` | GUID (FK) | |
| `Symbol` | NVARCHAR | Asset cần theo dõi |
| `TargetPrice` | DECIMAL | Mức giá mục tiêu |
| `Condition` | ENUM | 0 = GreaterThan, 1 = LessThan |
| `IsActive` | BOOL | Trạng thái bật/tắt |

---

## 4. 🔌 THIẾT KẾ API (API CONTRACT)

Tất cả API dưới đây yêu cầu Header: `Authorization: Bearer <token>`

### 🔐 Auth Group
* `POST /api/auth/register`: Đăng ký tài khoản.
* `POST /api/auth/login`: Đăng nhập, nhận JWT.

### 💰 Portfolio Group
* `GET /api/portfolio`: Lấy danh sách tài sản và tổng PnL của User.
* `POST /api/transactions`: Thêm giao dịch Mua/Bán mới.
    * *Logic Backend:* Khi thêm Transaction -> Tự động tính lại `AvgBuyPrice` và `TotalQuantity` trong bảng Portfolio.
* `GET /api/transactions/{symbol}`: Xem lịch sử giao dịch của 1 coin.

### 🔔 Alert Group
* `GET /api/alerts`: Danh sách cảnh báo đã cài.
* `POST /api/alerts`: Tạo cảnh báo mới.
* `DELETE /api/alerts/{id}`: Xóa cảnh báo.

---

## 5. 🛠️ NÂNG CẤP KỸ THUẬT (TECH STACK UPGRADE)

Để đáp ứng tính năng mới và chịu tải tốt hơn, cần nâng cấp các công nghệ sau:

1.  **Redis (Caching):**
    * *Mục đích:* Lưu giá coin mới nhất từ CoinCap API.
    * *Lợi ích:* API `GET /api/assets` sẽ đọc từ RAM (Redis) thay vì query SQL Server, giảm độ trễ xuống < 5ms.

2.  **SignalR Groups:**
    * *Mục đích:* Gửi thông báo riêng tư (Alert).
    * *Cơ chế:* Khi User A đăng nhập, add ConnectionId vào Group `User_A`. Khi có cảnh báo, chỉ gửi tin nhắn đến Group đó.

3.  **Docker & Docker Compose:**
    * Đóng gói toàn bộ hệ thống (WebAPI + SQL Server + Redis) thành các container để dễ dàng triển khai.

---

## 6. 📅 LỘ TRÌNH THỰC HIỆN (ROADMAP)

### Sprint 1: Foundation & Auth (Tuần 1)
- [ ] Thiết lập Identity DbContext.
- [ ] Implement JWT Authentication Service.
- [ ] API Register/Login.
- [ ] Update Database Migrations.

### Sprint 2: Portfolio Core (Tuần 2)
- [ ] Implement Repository cho Portfolio & Transaction.
- [ ] Viết Business Logic tính toán PnL (Profit and Loss).
- [ ] API CRUD Transactions.

### Sprint 3: Performance & Alerts (Tuần 3)
- [ ] Tích hợp Redis Caching cho giá Coin.
- [ ] Nâng cấp Worker Service: Kiểm tra điều kiện Alert mỗi khi có giá mới.
- [ ] Implement SignalR Private Notification.

### Sprint 4: Frontend Upgrade (Tuần 4)
- [ ] Tạo trang Login/Register.
- [ ] Dashboard cá nhân: Bảng danh mục đầu tư, Biểu đồ tròn (Asset Allocation).
- [ ] UI quản lý Cảnh báo giá.
