using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.DTOs
{
    // Class CreateAssetDto: Chỉ chứa những thông tin cần thiết khi tạo mới tài sản
    public class CreateAssetDto
    {
        // Mã giao dịch là bắt buộc khi tạo mới
        public string Symbol { get; set; } = string.Empty;

        // Tên tài sản là bắt buộc
        public string Name { get; set; } = string.Empty;
    }
}