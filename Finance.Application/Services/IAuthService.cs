using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Application.DTOs; // Import DTO

namespace Finance.Application.Services
{
    // Interface quy định các chức năng xác thực
    public interface IAuthService
    {
        // Hàm đăng ký: Nhận vào RegisterDto, trả về true nếu thành công, false nếu thất bại (kèm thông báo lỗi)
        // Tuple (bool, string) nghĩa là: (Thành công/Thất bại, "Thông báo lỗi nếu có")
        Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterDto registerDto);

        // Hàm đăng nhập: Nhận vào LoginDto, trả về AuthResponseDto (chứa Token) nếu thành công
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    }
}