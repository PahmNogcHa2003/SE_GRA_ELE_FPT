using Application.DTOs;
using Application.DTOs.Tickets;
using AutoMapper;
using Domain.Entities;
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
            CreateMap<Wallet, WalletDTO>().ReverseMap();
            CreateMap<WalletTransaction, WalletTransactionDTO>().ReverseMap();
            CreateMap<CreateTicketPlanDTO, TicketPlan>();
            CreateMap<UpdateTicketPlanDTO, TicketPlan>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<TicketPlan, TicketPlanDTO>().ReverseMap();
            CreateMap<CreateTicketPlanPriceDTO, TicketPlanPrice>();
            CreateMap<UpdateTicketPlanPriceDTO, TicketPlanPrice>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<TicketPlanPrice, TicketPlanPriceDTO>().ReverseMap();
            CreateMap<UserTicket, UserTicketDTO>().ReverseMap();
            CreateMap<UserTicket, ManageUserTicketDTO>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email)) 
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => $"{src.PlanPrice.Plan.Name} - {src.PlanPrice.VehicleType}"));
            CreateMap<Domain.Entities.UserProfile, DTOs.UserProfileDTO>().ReverseMap();
            CreateMap<Domain.Entities.Province, DTOs.Location.LocationDTO>().ReverseMap();
            CreateMap<Domain.Entities.Ward, DTOs.Location.LocationDTO>().ReverseMap();
            CreateMap<Domain.Entities.Rental, DTOs.RentalDTO>().ReverseMap();
            CreateMap<Domain.Entities.UserDevice, DTOs.UserDevice.UserDeviceDTO>().ReverseMap();
            CreateMap<Domain.Entities.Contact, DTOs.Contact.CreateContactDTO>().ReverseMap();
            CreateMap<Domain.Entities.Contact, DTOs.Contact.ReplyContactDTO>().ReverseMap();
            CreateMap<Domain.Entities.Contact, DTOs.Contact.ManageContactDTO>().ReverseMap();



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
