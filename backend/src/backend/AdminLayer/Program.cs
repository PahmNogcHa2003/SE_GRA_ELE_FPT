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

// 1. MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

// 2. Infrastructure (DbContext, Repo, Redis, ...)
builder.Services.AddInfrastructure(builder.Configuration);

// 3. Identity
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

// 4. Controllers + custom validation
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

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy
            .SetIsOriginAllowed(origin => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});


// 6. Authentication + JWT  (có log events)
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

    // LOG lỗi auth để debug
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine("AUTH FAILED: " + ctx.Exception.Message);
            return Task.CompletedTask;
        },
        OnForbidden = ctx =>
        {
            Console.WriteLine("AUTH FORBIDDEN (403): user not allowed.");
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            Console.WriteLine("AUTH CHALLENGE (401).");
            return Task.CompletedTask;
        }
    };
});

// 7. Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Admin", "Staff"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddHttpContextAccessor();

// 8. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AdminLayer API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 9. Auto migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HolaBikeContext>();
    db.Database.Migrate();
}

// 10. Seed roles + admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();
    var userManager = services.GetRequiredService<UserManager<AspNetUser>>();

    string[] roles = { "Admin", "Staff", "User" };

    foreach (var r in roles)
        if (!await roleManager.RoleExistsAsync(r))
            await roleManager.CreateAsync(new IdentityRole<long>(r));

    var adminEmail = "admin@ecojourney.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new AspNetUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true,
            PhoneNumber = "0123456789",
            SecurityStamp = Guid.NewGuid().ToString(),
            CreatedDate = DateTimeOffset.UtcNow
        };

        var result = await userManager.CreateAsync(admin, "Admin@123");
        if (result.Succeeded)
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Staff" });
    }
}

// 11. Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ValidationMiddleware>();


app.UseRouting();
app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
