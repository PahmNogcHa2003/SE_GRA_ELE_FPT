using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Identity;
using Application.Interfaces.Location;
using Application.Interfaces.Photo;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Mapping;
using Application.Services.Base;
using Application.Services.Identity;
using Application.Services.Staff;
using Application.Services.User;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Location;
using Infrastructure.Repositories.Photo;
using Infrastructure.Repositories.Staff;
using Infrastructure.Repositories.User;
using Infrastructure.Setting;
using Infrastructure.VNPay;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Dependency_Injection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // --- DbContext ---
            services.AddDbContext<HolaBikeContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // --- Settings / Configurations ---
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // --- Unit of Work & Repositories ---
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));

            // Repositories cụ thể
            services.AddScoped<IRepository<Station, long>, StationsRepository>();
            services.AddScoped<IRepository<CategoriesVehicle, long>, CategoriesVehicleRepository>();
            services.AddScoped<IRepository<Vehicle, long>, VehicleRepository>();
            services.AddScoped<IRepository<Tag, long>, TagRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletDebtRepository, WalletDebtRepository>();
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IManageTicketPlanRepository, ManageTicketPlanRepository>();
            services.AddScoped<ITicketPlanPriceRepository, TicketPlanPriceRepository>();
            services.AddScoped<IUserTicketRepository, UserTicketRepository>();
            services.AddScoped<IManageUserTicketRepository, ManageUserTicketRepository>();
            services.AddScoped<IRepository<Rental, long>, RentalsRepository>();
            services.AddScoped<IRepository<UserDevice, long>, UserDevicesRepository>();
            services.AddScoped<IKycRepository, KycRepository>();

            // --- Cloud Services / External Repositories ---
            services.AddScoped<IPhotoRepository, PhotoRepository>();

            // --- Location API & HTTP Client Factory ---
            // Thêm HttpClientFactory để các service khác (như OcrSpaceRepository) có thể sử dụng
            services.AddHttpClient();
            services.AddHttpClient("ProvincesAPI", client =>
            {
                client.BaseAddress = new Uri("https://provinces.open-api.vn/api/v2/");
            });
            services.AddMemoryCache();
            services.AddScoped<ILocationRepository, LocationRepository>();

            // --- Services (application layer) ---
            services.AddScoped(typeof(IService<,,>), typeof(GenericService<,,>));
            services.AddScoped<IStationsService, StationsService>();
            services.AddScoped<ICategoriesVehicleService, CategoriesVehicleService>();
            services.AddScoped<IVehicleService, VehiclesService>();
            services.AddScoped<INewsService, NewsService>();
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
            services.AddScoped<IUserDevicesService, UserDevicesService>();
            services.AddScoped<IKycService, KycService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // --- AutoMapper ---
            services.AddAutoMapper(typeof(AppMappingProfile).Assembly);

            //  --- Add AuthService (Identity + JWT) ---
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}

