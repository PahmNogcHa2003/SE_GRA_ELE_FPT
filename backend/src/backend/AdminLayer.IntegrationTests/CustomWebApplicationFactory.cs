using AdminLayer;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace AdminLayer.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // ⭐ SỬA LẠI PHẦN NÀY ĐỂ XÓA TRIỆT ĐỂ CẤU HÌNH CŨ ⭐
                // =================================================================
                // 1. Tìm và xóa DbContextOptions registration
                var dbContextOptionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<HolaBikeContext>));

                if (dbContextOptionsDescriptor != null)
                {
                    services.Remove(dbContextOptionsDescriptor);
                }

                // 2. Tìm và xóa DbContext registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(HolaBikeContext));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }
                // =================================================================


                // 3. Đăng ký lại DbContext, nhưng trỏ tới một In-Memory Database
                services.AddDbContext<HolaBikeContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // 4. Lấy service provider và seed dữ liệu mẫu
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<HolaBikeContext>();

                    db.Database.EnsureCreated();

                    // Seed data... (phần này giữ nguyên)
                    db.Stations.RemoveRange(db.Stations);
                    db.SaveChanges();
                    db.Stations.AddRange(
                         new Station { Id = 1, Name = "Ga Sài Gòn", Location = "1 Nguyễn Thông, Quận 3, TP.HCM", IsActive = true, Lat = 10.78m, Lng = 106.68m },
                         new Station { Id = 2, Name = "Ga Hà Nội", Location = "120 Lê Duẩn, Hoàn Kiếm, Hà Nội", IsActive = true, Lat = 21.02m, Lng = 105.84m }
                    );
                    db.SaveChanges();
                }
            });
        }
    }
}