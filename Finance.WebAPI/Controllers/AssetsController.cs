using Finance.Application.DTOs; // Import DTO
using Finance.Application.Services; // Import Service Interface
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; // Import thư viện tạo API

// Namespace Controllers
namespace Finance.WebAPI.Controllers
{
    // [ApiController]: Đánh dấu class này là API Controller
    // [Route("api/[controller]")]: Định nghĩa đường dẫn URL. 
    // "Assets" lấy từ tên class (bỏ chữ Controller). -> URL: /api/assets
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssetsController : ControllerBase
    {
        // Khai báo Service để xử lý logic
        private readonly IAssetService _assetService;

        // Constructor Injection: Nhận Service từ DI Container
        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        // 1. API Lấy danh sách tất cả tài sản
        // GET: api/assets
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Gọi service lấy dữ liệu
            var assets = await _assetService.GetAllAssetsAsync();
            // Trả về HTTP 200 (OK) kèm dữ liệu
            return Ok(assets);
        }

        // 2. API Lấy chi tiết tài sản theo ID
        // GET: api/assets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null)
            {
                // Nếu không thấy, trả về HTTP 404 (Not Found)
                return NotFound();
            }
            return Ok(asset);
        }

        // 3. API Tạo mới tài sản
        // POST: api/assets
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssetDto createAssetDto)
        {
            try
            {
                // Gọi service tạo mới
                var createdAsset = await _assetService.CreateAssetAsync(createAssetDto);

                // Trả về HTTP 201 (Created)
                // Kèm theo Header "Location" trỏ đến API xem chi tiết tài sản vừa tạo
                return CreatedAtAction(nameof(GetById), new { id = createdAsset.Id }, createdAsset);
            }
            catch (InvalidOperationException ex)
            {
                // Nếu lỗi logic (ví dụ trùng Symbol), trả về 400 Bad Request
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. API Lấy lịch sử giá của tài sản
        // GET: api/assets/5/history
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(int id)
        {
            var history = await _assetService.GetAssetHistoryAsync(id);
            return Ok(history);
        }

        //API Xóa tài sản
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _assetService.DeleteAssetAsync(id);
            return NoContent(); // Trả về 204 (Thành công nhưng không có nội dung trả về)
        }
    }
}