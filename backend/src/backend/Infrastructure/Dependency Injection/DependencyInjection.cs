using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Identity;
using Application.Mapping;
using Application.Services.Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services.Identity;
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

            // --- Services (application layer) ---
            services.AddScoped(typeof(IService<,,>), typeof(GenericService<,,>));
            //services.AddScoped<IUserService, UserService>();

            // --- AutoMapper ---
            services.AddAutoMapper(typeof(AppMappingProfile).Assembly);

            //  Add AuthService (Identity + JWT)
            services.AddScoped<IAuthService, AuthService>();


            return services;
        }
    }
}
