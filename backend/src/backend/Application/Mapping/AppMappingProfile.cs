using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            // Example mapping configuration
            // CreateMap<Source, Destination>();
            CreateMap<Domain.Entities.AspNetUser, DTOs.UserDTO>().ReverseMap();
            CreateMap<Domain.Entities.Station, DTOs.StationDTO>().ReverseMap();
            CreateMap<Domain.Entities.CategoriesVehicle, DTOs.CategoriesVehicleDTO>().ReverseMap();
            CreateMap<Domain.Entities.Vehicle, DTOs.VehicleDTO>().ReverseMap();
            CreateMap<Domain.Entities.Tag, DTOs.TagDTO>().ReverseMap();

            ConfigureNewsMapping();

        }

        private void ConfigureNewsMapping()
        {
            // Chiều từ Entity -> DTO (Khi lấy dữ liệu ra)
            CreateMap<Domain.Entities.News, DTOs.NewsDTO>()
                .ForMember(dest => dest.TagIds,
                           opt => opt.MapFrom(src => src.TagNews.Select(t => t.Id).ToList()));

            // === DÒNG QUAN TRỌNG NHẤT ĐỂ SỬA LỖI ===
            // Chiều từ DTO -> Entity (Khi tạo mới/cập nhật)
            // BẮT BUỘC Bỏ qua (Ignore) việc map thuộc tính Tags.
            CreateMap<DTOs.NewsDTO, Domain.Entities.News>()
                .ForMember(dest => dest.TagNews, opt => opt.Ignore());
        }
    }
}
