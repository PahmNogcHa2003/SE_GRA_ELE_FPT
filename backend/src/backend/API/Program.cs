using Application.Interfaces;
using Application.Mapping;
using Application.Services;
using Domain.Entities;
using Infrastructure.Dependency_Injection;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Infrastructure (DbContext, Repo, UnitOfWork,...)
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// Swagger (default)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AppMappingProfile).Assembly);

// DbContext
builder.Services.AddDbContext<HolaBikeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories (generic + custom)
builder.Services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
builder.Services.AddScoped<IRepository<AspNetUser, long>, UserRepository>();

// Services (generic + custom)
builder.Services.AddScoped(typeof(IService<,,>), typeof(GenericService<,,>));
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
