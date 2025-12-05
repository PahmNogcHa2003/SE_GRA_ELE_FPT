using Application.DTOs;
using Application.DTOs.CategoriesVehicle;
using Application.DTOs.Kyc;
using Application.DTOs.New;
using Application.DTOs.Promotion;
using Application.DTOs.Quest;
using Application.DTOs.Station;
using Application.DTOs.Tag;
using Application.DTOs.TagNew;
using Application.DTOs.Tickets;
using Application.DTOs.User;
using Application.DTOs.Wallet;
using Application.DTOs.WalletTransaction;
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
            CreateMap<Domain.Entities.AspNetUser, UserDTO>().ReverseMap();
            CreateMap<Domain.Entities.Station, StationDTO>().ReverseMap();
            CreateMap<Domain.Entities.CategoriesVehicle, CategoriesVehicleDTO>().ReverseMap();
            CreateMap<Domain.Entities.Vehicle, DTOs.Vehicle.VehicleDTO>().ReverseMap();
            CreateMap<Domain.Entities.Tag, TagDTO>().ReverseMap();
            CreateMap<Wallet, WalletDTO>().ReverseMap();
            CreateMap<News, NewsDTO>()
            .ForMember(dest => dest.TagIds,
               opt => opt.MapFrom(src => src.TagNews.Select(t => t.TagId).ToList()));

            CreateMap<TagNew, TagNewDTO>().ReverseMap();
            CreateMap<WalletTransaction, WalletTransactionDTO>().ReverseMap();
            CreateMap<CreateTicketPlanDTO, TicketPlan>();
            CreateMap<UpdateTicketPlanDTO, TicketPlan>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<TicketPlan, TicketPlanDTO>().ReverseMap();
            CreateMap<CreateTicketPlanPriceDTO, TicketPlanPrice>();
            CreateMap<UpdateTicketPlanPriceDTO, TicketPlanPrice>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<TicketPlanPrice, TicketPlanPriceDTO>().ReverseMap();
            CreateMap<UserTicket, UserTicketDTO>()
                .ForMember(d => d.PlanName, m => m.MapFrom(s => s.PlanPrice.Plan.Name))
                .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.PlanPrice.VehicleType))
                .ForMember(d => d.ActivationMode, m => m.MapFrom(s => (ActivationModeDTO)(int)s.PlanPrice.ActivationMode));
            CreateMap<UserTicket, ManageUserTicketDTO>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email)) 
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => $"{src.PlanPrice.Plan.Name} - {src.PlanPrice.VehicleType}"));
            CreateMap<Domain.Entities.UserProfile, DTOs.UserProfile.UserProfileDTO>().ReverseMap();
            CreateMap<Domain.Entities.Province, DTOs.Location.LocationDTO>().ReverseMap();
            CreateMap<Domain.Entities.BookingTicket, DTOs.BookingTicket.CreateBookingTicketDTO>().ReverseMap();
            CreateMap<Domain.Entities.Ward, DTOs.Location.LocationDTO>().ReverseMap();
            CreateMap<Domain.Entities.Rental, DTOs.RentalDTO>().ReverseMap();
            CreateMap<Domain.Entities.UserDevice, DTOs.UserDevice.UserDeviceDTO>().ReverseMap();
            CreateMap<TicketPlanPrice, UserTicketPlanPriceDTO>()
                .ForMember(d => d.ActivationMode, m => m.MapFrom(s => (ActivationModeDTO)(int)s.ActivationMode));
            CreateMap<TicketPlan, UserTicketPlanDTO>();
            CreateMap<Domain.Entities.Contact, DTOs.Contact.CreateContactDTO>().ReverseMap();
            CreateMap<Domain.Entities.Contact, DTOs.Contact.ReplyContactDTO>().ReverseMap();
            CreateMap<Domain.Entities.Contact, DTOs.Contact.ManageContactDTO>().ReverseMap();
            CreateMap<Domain.Entities.KycForm, DTOs.Kyc.CreateKycRequestDTO>().ReverseMap();
            CreateMap<PromotionCampaign, PromotionDTO>().ReverseMap();
            CreateMap<PromotionCreateDTO, PromotionCampaign>();
            CreateMap<Quest, QuestDTO>();
            CreateMap<QuestCreateDTO, Quest>();
            CreateMap<QuestUpdateDTO, Quest>();
            CreateMap<TagNewDTO, TagNew>().ReverseMap();
        }
    }
}
