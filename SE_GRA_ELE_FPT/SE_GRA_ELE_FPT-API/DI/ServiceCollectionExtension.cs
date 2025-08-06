using Autofac;
using Autofac.Extensions.DependencyInjection;
using SE_GRA_ELE_FPT_DBAccess;
using SE_GRA_ELE_FPT_DBAccess.UnitOfWork;


namespace SE_GRA_ELE_FPT_API.DI
{
    public static class ServiceCollectionExtension
    {

        public static void AddServiceCollection(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
        {
            // Call UseServiceProviderFactory on the Host sub property 
            host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            services.AddMemoryCache();

            // Call ConfigureContainer on the Host sub property 
            host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
                //builder.RegisterType<CustomOTPTokenProvider<Users>>().AsSelf();
                builder.RegisterType<ApplicationDbContext>().AsSelf();
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
                //builder.RegisterType<AuthService>().As<IAuthService>();
            });
        }
    }
}
