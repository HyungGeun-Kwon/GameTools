using GameTools.Api;
using GameTools.Application.Abstractions.Users;
using GameTools.Application.Extensions;
using GameTools.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, ApiUser>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// dotnet-ef 설치
// dotnet tool install --global dotnet-ef

// 마이그레이션
// dotnet ef migrations add 마이그레이션이름 -p GameTools.Infrastructure/GameTools.Infrastructure.csproj -s GameTools.Api/GameTools.Api.csproj
// dotnet ef migrations add FixRowVersionConcurrency -p GameTools.Infrastructure/GameTools.Infrastructure.csproj -s GameTools.Api/GameTools.Api.csproj

// DB에 반영
// dotnet ef database update -p GameTools.Infrastructure/GameTools.Infrastructure.csproj -s GameTools.Api/GameTools.Api.csproj
