using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Identity;
using Application.Interfaces.Location;
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
using Infrastructure.Repositories.Staff;
using Infrastructure.Repositories.User;
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
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IRepository<Rental, long>, RentalsRepository>();

            // --- Location API ---
            services.AddHttpClient("ProvincesAPI", client =>
            {
                client.BaseAddress = new Uri("https://provinces.open-api.vn/api/v2/");
            });
            //  cache
            services.AddMemoryCache();
            //LocationRepository
            services.AddScoped<ILocationRepository, LocationRepository>();

            // --- Services (application layer) ---
            services.AddScoped(typeof(IService<,,>), typeof(GenericService<,,>));
            //services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStationsService, StationsService>();
            services.AddScoped<ICategoriesVehicleService, CategoriesVehicleService>();
            services.AddScoped<IVehicleService, VehiclesService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IUserProfilesService, UserProfilesService>();
            services.AddScoped<IRentalsService, RentalsService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();




            // --- AutoMapper ---
            services.AddAutoMapper(typeof(AppMappingProfile).Assembly);

            //  Add AuthService (Identity + JWT)
            services.AddScoped<IAuthService, AuthService>();


            return services;
        }
    }
}