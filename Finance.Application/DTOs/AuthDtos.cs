using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // Thư viện dùng để kiểm tra dữ liệu (Validate)

namespace Finance.Application.DTOs
{
    // DTO dùng cho việc Đăng ký tài khoản
    public class RegisterDto
    {
        [Required] // Bắt buộc phải có
        [EmailAddress] // Phải đúng định dạng Email
        public string Email { get; set; } = string.Empty;

        [Required] // Bắt buộc phải có
        [MinLength(6)] // Mật khẩu tối thiểu 6 ký tự
        public string Password { get; set; } = string.Empty;
    }

    // DTO dùng cho việc Đăng nhập
    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // DTO trả về sau khi đăng nhập thành công
    public class AuthResponseDto
    {
        // Token chuỗi dùng để xác thực các request sau này
        public string Token { get; set; } = string.Empty;

        // Email của người dùng
        public string Email { get; set; } = string.Empty;

        // Thời gian hết hạn của token (để Client biết khi nào cần login lại)
        public DateTime Expiration { get; set; }
    }
}