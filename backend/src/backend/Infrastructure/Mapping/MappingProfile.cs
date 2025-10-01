using AutoMapper;

namespace Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map 2 chiều giữa EF Model <-> Domain Entity
            CreateMap<Infrastructure.Persistence.Models.Booking, Domain.Entities.Models.Booking>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.BookingTicket, Domain.Entities.Models.BookingTicket>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.CategoriesVehicle, Domain.Entities.Models.CategoriesVehicle>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.Contact, Domain.Entities.Models.Contact>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.KycDocument, Domain.Entities.Models.KycDocument>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.KycProfile, Domain.Entities.Models.KycProfile>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.KycSubmission, Domain.Entities.Models.KycSubmission>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.News, Domain.Entities.Models.News>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.Order, Domain.Entities.Models.Order>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.OrderItem, Domain.Entities.Models.OrderItem>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.Payment, Domain.Entities.Models.Payment>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.Rental, Domain.Entities.Models.Rental>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.Station, Domain.Entities.Models.Station>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.StationLog, Domain.Entities.Models.StationLog>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.Tag, Domain.Entities.Models.Tag>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.TicketPlan, Domain.Entities.Models.TicketPlan>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.TicketPlanPrice, Domain.Entities.Models.TicketPlanPrice>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.UserAddress, Domain.Entities.Models.UserAddress>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.UserDevice, Domain.Entities.Models.UserDevice>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.UserProfile, Domain.Entities.Models.UserProfile>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.UserSession, Domain.Entities.Models.UserSession>()
                .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.UserTicket, Domain.Entities.Models.UserTicket>()
               .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.Vehicle, Domain.Entities.Models.Vehicle>()
               .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.VehicleUsageLog, Domain.Entities.Models.VehicleUsageLog>()
               .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.Wallet, Domain.Entities.Models.Wallet>()
               .ReverseMap();

            CreateMap<Infrastructure.Persistence.Models.WalletTransaction, Domain.Entities.Models.WalletTransaction>()
               .ReverseMap();
        }
    }
}
