using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Factory cho phép EF Core tạo DbContext khi chạy lệnh Add-Migration hoặc Update-Database.
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<HolaBikeContext>
    {
        public HolaBikeContext CreateDbContext(string[] args)
        {
            // ✅ 1. Tìm appsettings.json trong API project (tự động xác định)
            string basePath = GetApiProjectPath();

            Console.WriteLine($"🔍 [AppDbContextFactory] Loading appsettings.json from: {basePath}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // ✅ 2. Lấy connection string từ appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("❌ Connection string 'DefaultConnection' not found in appsettings.json!");
            }

            // ✅ 3. Build DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<HolaBikeContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new HolaBikeContext(optionsBuilder.Options);
        }

        /// <summary>
        /// Hàm phụ tìm đúng đường dẫn tới project API (chứa appsettings.json)
        /// </summary>
        private string GetApiProjectPath()
        {
            // Khi chạy migration, EF đặt CurrentDirectory ở bin/Debug/... => cần đi lên 3 cấp
            var currentDir = Directory.GetCurrentDirectory();

            // Nếu đang chạy CLI: chạy lệnh EF có --startup-project thì lấy luôn thư mục đó
            if (Directory.Exists(Path.Combine(currentDir, "appsettings.json")))
                return currentDir;

            // Ngược lại: thử tìm thư mục "APIUserLayer" trong solution
            var possiblePath = Path.Combine(currentDir, "../APIUserLayer");

            if (Directory.Exists(possiblePath))
                return Path.GetFullPath(possiblePath);

            // Nếu chạy trong thư mục bin, phải đi lên nhiều cấp
            var deepPath = Path.Combine(currentDir, "../../../APIUserLayer");
            if (Directory.Exists(deepPath))
                return Path.GetFullPath(deepPath);

            throw new DirectoryNotFoundException("❌ Không tìm thấy thư mục chứa appsettings.json (APIUserLayer).");
        }
    }
}
