using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper; // Sử dụng thư viện AutoMapper
using Finance.Application.DTOs; // Import các DTO vừa tạo
using Finance.Domain.Entities; // Import các Entity từ Domain

// Namespace cho Mappings
namespace Finance.Application.Mappings
{
    // Class MappingProfile kế thừa từ Profile của AutoMapper
    public class MappingProfile : Profile
    {
        // Constructor: Nơi định nghĩa các quy tắc map
        public MappingProfile()
        {
            // Map từ Entity Asset sang AssetDto
            // ReverseMap() cho phép map ngược lại từ DTO sang Entity nếu cần
            CreateMap<Asset, AssetDto>().ReverseMap();

            // Map từ CreateAssetDto sang Entity Asset (dùng khi tạo mới)
            CreateMap<CreateAssetDto, Asset>();

            // Map từ Entity PriceHistory sang PriceHistoryDto
            CreateMap<PriceHistory, PriceHistoryDto>();
        }
    }
}