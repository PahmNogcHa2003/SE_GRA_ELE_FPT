using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Email;
using Application.Interfaces.Identity;
using Application.Interfaces.Location;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Mapping;
using Application.Services.Base;
using Application.Services.Email;
using Application.Services.Identity;
using Application.Services.Staff;
using Application.Services.User;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Email;
using Infrastructure.Repositories.Location;
using Infrastructure.Repositories.Staff;
using Infrastructure.Repositories.User;
using Infrastructure.VNPay;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dependency_Injection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // --- DbContext ---
            services.AddDbContext<HolaBikeContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // --- Unit of Work & Repositories ---
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
            //services.AddScoped<IRepository<AspNetUser, long>, UserRepository>();
            services.AddScoped<IRepository<Station, long>, StationsRepository>();
            services.AddScoped<IRepository<CategoriesVehicle, long>, CategoriesVehicleRepository>();
            services.AddScoped<IRepository<Vehicle, long>, VehicleRepository>();
            services.AddScoped<IRepository<Tag, long>, TagRepository>();
            services.AddScoped<Application.Interfaces.Staff.Repository.INewsRepository, Repositories.Staff.NewsRepository>();
            services.AddScoped<Application.Interfaces.User.Repository.INewsRepository, Repositories.User.NewsRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletDebtRepository, WalletDebtRepository>();
            services.AddScoped<Application.Interfaces.Staff.Repository.IWalletTransactionRepository, Repositories.Staff.WalletTransactionRepository>();
            services.AddScoped<Application.Interfaces.User.Repository.IWalletTransactionRepository, Repositories.User.WalletTransactionRepository>();
            services.AddScoped<IManageTicketPlanRepository, ManageTicketPlanRepository>();
            services.AddScoped<ITicketPlanPriceRepository, TicketPlanPriceRepository>();
            services.AddScoped<IUserTicketRepository, UserTicketRepository>();
            services.AddScoped<IManageUserTicketRepository, ManageUserTicketRepository>();
            services.AddScoped<IRepository<Rental, long>, RentalsRepository>();
            services.AddScoped<IRepository<UserDevice, long>, UserDevicesRepository>();
            services.AddScoped<IEmailRepository, MailRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IManageContactRepository, ManageContactRepository>();
            services.AddScoped<IReplyContactRepository, ReplyContactRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();


            // --- Location API ---
            services.AddHttpClient("ProvincesAPI", client =>
            {
                client.BaseAddress = new Uri("https://provinces.open-api.vn/api/v2/");
            });
            //  cache
            services.AddMemoryCache();
            //LocationRepository
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.Configure<Setting.MailSettings>(config.GetSection("MailSettings"));

            // --- Services (application layer) ---
            services.AddScoped(typeof(IService<,,>), typeof(GenericService<,,>));
            //services.AddScoped<IUserService, UserService>();
            services.AddScoped<Application.Interfaces.Staff.Service.IStationsService, Application.Services.Staff.StationsService>();
            services.AddScoped<Application.Interfaces.User.Service.IStationsService, Application.Services.User.StationsService>();
            services.AddScoped<ICategoriesVehicleService, CategoriesVehicleService>();
            services.AddScoped<IVehicleService, VehiclesService>();
            services.AddScoped<Application.Interfaces.User.Service.INewsService, Application.Services.User.NewsService>();
            services.AddScoped<Application.Interfaces.Staff.Service.INewsService, Application.Services.Staff.NewsService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IPaymentService, PaymentService>(); 
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IPaymentGatewayService, VnPayService>();
            services.AddScoped<IUserWalletService, UserWalletService>();
            services.AddScoped<IWalletTransactionService, WalletTransactionService>();
            services.AddScoped<IManageTicketPlanService, ManageTicketPlanSevice>();
            services.AddScoped<ITicketPlanPriceService, TicketPlanPriceService>();
            services.AddScoped<IUserTicketService, UserTicketService>();
            services.AddScoped<IManageUserTicketService, ManageUserTicketService>();
            services.AddScoped<IUserProfilesService, UserProfilesService>();
            services.AddScoped<IRentalsService, RentalsService>();
            services.AddScoped<IRentalsService, RentalsService>();
            services.AddScoped<IUserDevicesService, UserDevicesService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IManageContactService, ManageContactService>();
            services.AddScoped<IReplyContactService, ReplyContactService>();
            services.AddScoped<ITransactionService, TransactionService>();

            // --- AutoMapper ---
            services.AddAutoMapper(typeof(AppMappingProfile).Assembly);

            //  Add AuthService (Identity + JWT)
            services.AddScoped<IAuthService, AuthService>();


            return services;
        }
    }
}