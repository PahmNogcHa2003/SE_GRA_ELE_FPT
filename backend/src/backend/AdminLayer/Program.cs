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
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

// Infrastructure (DB, Repo, Services...) - ĐẢM BẢO KHÔNG CÓ AddSwaggerGen Ở TRONG NÀY
builder.Services.AddInfrastructure(builder.Configuration);

// Identity
builder.Services.AddIdentity<AspNetUser, IdentityRole<long>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<HolaBikeContext>()
.AddDefaultTokenProviders()
.AddRoleManager<RoleManager<IdentityRole<long>>>();

// Controllers + Validation response
builder.Services.AddControllers()
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

// CORS: CHỌN 1 policy dùng thật sự
builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()
));

// Chỉ GỌI 1 LẦN
builder.Services.AddEndpointsApiExplorer();

// AuthN + JWT

// Add Authentication & JWT
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

// AuthZ policies: CHỈ KHAI BÁO 1 LẦN
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Admin", "Staff"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// Swagger: CHỈ KHAI BÁO 1 LẦN Ở ĐÂY
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AdminLayer API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seeding: GỘP THÀNH 1 KHỐI (tránh tạo 2 admin khác nhau)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();
        var userManager = services.GetRequiredService<UserManager<AspNetUser>>();

        string[] roles = { "Admin", "Staff", "User" };
        foreach (var r in roles)
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole<long>(r));

        var adminEmail = "admin@ecojourney.com";   // CHỌN 1 email, tránh trùng
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AspNetUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumber = "0123456789",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.UtcNow
            };
            var create = await userManager.CreateAsync(adminUser, "Admin@123");
            if (!create.Succeeded)
                throw new Exception("Create admin failed: " + string.Join(", ", create.Errors.Select(e => e.Description)));

            var addRoles = await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Staff" });
            if (!addRoles.Succeeded)
                throw new Exception("Add roles failed: " + string.Join(", ", addRoles.Errors.Select(e => e.Description)));
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Seeding identity failed.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // 1 lần
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<ResponseMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

// CHỈ GỌI 1 LẦN CORS VỚI POLICY BẠN MUỐN
app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
