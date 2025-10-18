using Application.Common;
using Domain.Entities;
using Infrastructure.Dependency_Injection;
using Infrastructure.Middlewares;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection; // ✅ 1. THÊM USING CHO MEDIATR
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅✅✅ 2. ĐĂNG KÝ MEDIATR - DÒNG QUAN TRỌNG NHẤT ĐỂ SỬA LỖI ✅✅✅
// Dòng này phải được thêm vào để hệ thống biết về các Query, Command, Handler.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

// -------------------- Infrastructure Layer --------------------
// Dòng này gọi vào file DependencyInjection.cs để đăng ký DbContext, Repositories, Services, HttpClient, AutoMapper...
builder.Services.AddInfrastructure(builder.Configuration);

// -------------------- Identity Config (Web Layer) --------------------
builder.Services.AddIdentity<AspNetUser, IdentityRole<long>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<HolaBikeContext>()
.AddDefaultTokenProviders();

// -------------------- Controllers, ApiBehaviorOptions & Swagger --------------------
builder.Services.AddControllers() // (Đã gộp 2 lệnh AddControllers() làm một)
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage);

            var errorResponse = ApiResponse<object>.ErrorResponse(
                message: "Validation Failed",
                errors: errors
            );

            return new BadRequestObjectResult(errorResponse);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------- Authentication & JWT --------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


var app = builder.Build();

// -------------------- Middleware Pipeline --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Thứ tự Middleware rất quan trọng
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<ResponseMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
