CHẠY CODE
Lấy code về sau đó mở source bằng vs 2022
Kiểm tra file Dependencies của từng folder xem nó có các packages chưa ( nếu chưa chuột phải vào Solution -> chọn Restore NuGet Packages)
Mở file appsettings.json trong dự án Finance.WebAPI tại dòng ("DefaultConnection": "Server=localhost; Database=Base-103-Dev; User Id=sa; Password=Ntp080203; TrustServerCertificate=True")->("DefaultConnection": "Server=[tên sever];Database=FinanceRealtimeDb;Trusted_Connection=True;")
Sau đó vào tool trên thanh công cụ chọn NuGet Package Manager -> package manager console
->> ở phần Default project chọn Finance.Infrastructure sau đó gõ lệnh Update-Database
Có lỗi j báo tôi nha khoogn thì hỏi chat
