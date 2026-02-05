using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Identity; // Thư viện quản lý User
using Microsoft.IdentityModel.Tokens; // Thư viện tạo Token
using System.IdentityModel.Tokens.Jwt; // Thư viện xử lý JWT
using System.Security.Claims; // Thư viện xử lý thông tin trong Token (Claim)
using System.Text; // Xử lý text encoding

namespace Finance.WebAPI.Services
{
    // Thực thi interface IAuthService
    public class AuthService : IAuthService
    {
        // UserManager: Class có sẵn của Identity giúp thêm/sửa/xóa/check pass user
        private readonly UserManager<IdentityUser> _userManager;

        // IConfiguration: Giúp đọc file appsettings.json (để lấy Key bí mật)
        private readonly IConfiguration _configuration;

        // Constructor Injection
        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // --- XỬ LÝ ĐĂNG KÝ ---
        public async Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterDto registerDto)
        {
            // Kiểm tra xem email đã tồn tại chưa
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                return (false, "Email này đã được sử dụng!");

            // Tạo đối tượng User mới (IdentityUser)
            var user = new IdentityUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email, // Mặc định lấy Email làm UserName luôn
                SecurityStamp = Guid.NewGuid().ToString() // Một chuỗi ngẫu nhiên để tăng bảo mật
            };

            // Gọi hàm tạo user, Identity sẽ tự động mã hóa (Hash) password
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            // Nếu không thành công (VD: pass yếu, lỗi DB...)
            if (!result.Succeeded)
            {
                // Ghép các lỗi lại thành 1 chuỗi để trả về
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            return (true, "Đăng ký thành công!");
        }

        // --- XỬ LÝ ĐĂNG NHẬP ---
        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // 1. Tìm user trong DB theo Email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            // 2. Nếu không thấy user HOẶC password sai (CheckPasswordAsync tự so sánh hash)
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return null; // Trả về null báo hiệu đăng nhập thất bại
            }

            // 3. Nếu đúng hết, bắt đầu tạo Token JWT
            // --- BẮT ĐẦU TẠO TOKEN ---

            // Tạo các Claim (thông tin đính kèm trong token)
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName), // Lưu username
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Lưu User ID (quan trọng để xác định ai đang gọi API)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID riêng của Token này
            };

            // Lấy Key bí mật từ file cấu hình (appsettings.json)
            // Lưu ý: Key này phải dài ít nhất 16 ký tự để đủ độ mạnh
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            // Tạo object Token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"], // Ai phát hành (Server mình)
                audience: _configuration["JWT:ValidAudience"], // Ai sử dụng (Client)
                expires: DateTime.Now.AddHours(24), // Hết hạn sau 24h
                claims: authClaims, // Thông tin user
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256) // Chữ ký số bảo mật
            );

            // Trả về DTO chứa Token dạng chuỗi
            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token), // Chuyển object Token thành chuỗi string
                Email = user.Email,
                Expiration = token.ValidTo
            };
        }
    }
}