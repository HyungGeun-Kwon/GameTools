using GameTools.Server.Api;
using GameTools.Server.Application.Abstractions.Users;
using GameTools.Server.Application.Extensions;
using GameTools.Server.Infrastructure.Extensions;
using GameTools.Server.Infrastructure.Persistence;
using GameTools.Server.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
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

using (var scope = app.Services.CreateScope())
{
    // ���̱׷��̼�
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // �õ嵥����
    foreach (var seeder in sp.GetServices<ISeeder>())
        await seeder.SeedAsync(db, CancellationToken.None);
}

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// dotnet-ef ��ġ
// dotnet tool install --global dotnet-ef

// ���̱׷��̼�
// dotnet ef migrations add ���̱׷��̼��̸� -p GameTools.Server.Infrastructure/GameTools.Server.Infrastructure.csproj -s GameTools.Server.Api/GameTools.Server.Api.csproj

// DB�� �ݿ�
// dotnet ef database update -p GameTools.Server.Infrastructure/GameTools.Server.Infrastructure.csproj -s GameTools.Server.Api/GameTools.Server.Api.csproj
