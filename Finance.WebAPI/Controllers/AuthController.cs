using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.WebAPI.Controllers
{
    [Route("api/[controller]")] // URL sẽ là: /api/auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // API: POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // Gọi service xử lý đăng ký
            var result = await _authService.RegisterAsync(model);

            if (result.IsSuccess)
            {
                // Nếu thành công trả về 200 OK
                return Ok(new { message = result.Message });
            }

            // Nếu thất bại trả về 400 Bad Request kèm lỗi
            return BadRequest(new { message = result.Message });
        }

        // API: POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            // Gọi service xử lý đăng nhập
            var result = await _authService.LoginAsync(model);

            if (result != null)
            {
                // Nếu có kết quả (Token), trả về 200 OK
                return Ok(result);
            }

            // Nếu null (sai pass hoặc user), trả về 401 Unauthorized
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });
        }
    }
}