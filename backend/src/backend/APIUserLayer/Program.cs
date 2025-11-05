using Application.Common;
using Domain.Entities;
using Infrastructure.Dependency_Injection;
using Infrastructure.Middlewares;
using Infrastructure.Persistence;
using Infrastructure.Setting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Service Registration ---

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

// Add Infrastructure Layer (DbContext, Repositories, Services...)
builder.Services.AddInfrastructure(builder.Configuration);

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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


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

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Admin", "Staff"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

async Task SeedIdentityAsync(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<AspNetUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();

    string[] roles = { "Admin", "Staff", "User" };
    foreach (var r in roles)
    {
        if (!await roleManager.RoleExistsAsync(r))
            await roleManager.CreateAsync(new IdentityRole<long>(r));
    }

    string adminEmail = "admin@ecojourney.com";
    string adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AspNetUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true,
            PhoneNumber = "0123456789",
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var create = await userManager.CreateAsync(adminUser, adminPassword);
        if (!create.Succeeded)
            throw new Exception("Create admin failed: " + string.Join(", ", create.Errors.Select(e => e.Description)));

        var addRole = await userManager.AddToRoleAsync(adminUser, "Admin");
        if (!addRole.Succeeded)
            throw new Exception("Add role failed: " + string.Join(", ", addRole.Errors.Select(e => e.Description)));

        Console.WriteLine("✅ Admin user created and assigned Admin role.");
    }
    else
    {
        // đảm bảo đã có role
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            await userManager.AddToRoleAsync(adminUser, "Admin");

        Console.WriteLine("ℹ️ Admin user already exists.");
    }
}

// Add CORS
builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));

// Add Swagger with Auth support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HolaBike API", Version = "v1" });
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
            new string[] {}
        }
    });
});

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Admin", "Staff"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});
builder.Services.Configure<VnPaySettings>(builder.Configuration.GetSection("VnPaySettings"));
var app = builder.Build();

// Seed roles + admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedIdentityAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seeding identity failed.");
    }
}

// --- Middleware Pipeline Configuration ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<ResponseMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll"); // Use CORS Policy

app.UseAuthentication(); // This must come before UseAuthorization

app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// ✅ FIX: Make the auto-generated Program class public so test projects can access it.
// Make the Program class public so it can be accessed from test projects
public partial class Program { }

