using Infrastructure.Dependency_Injection;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Infrastructure (DbContext, Repo, UnitOfWork, Service, Mapper, ...)
builder.Services.AddInfrastructure(builder.Configuration);

// ✅ Add Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
