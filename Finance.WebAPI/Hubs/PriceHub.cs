using Microsoft.AspNetCore.SignalR; // Thư viện SignalR
using System.Threading.Tasks;

// Namespace Hubs
namespace Finance.WebAPI.Hubs
{
    // Class PriceHub kế thừa từ Hub
    // Đây là nơi quản lý các kết nối của client (Frontend)
    public class PriceHub : Hub
    {
        // Hiện tại chúng ta chưa cần xử lý logic gì phức tạp ở đây.
        // Chỉ cần class này tồn tại để server biết nơi "phát sóng" dữ liệu.

        // Ví dụ: Client có thể gọi hàm này để tham gia vào một nhóm coin cụ thể (nếu muốn mở rộng sau này)
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}