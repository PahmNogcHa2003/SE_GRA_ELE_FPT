using SE_GRA_ELE_FPT_API.DI;
using Serilog;
namespace SE_GRA_ELE_FPT_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("starting server.");
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                // Add dependency injection
                builder.Services.AddServiceCollection(builder.Configuration, builder.Host);

                // Add fluent validator
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "server terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
