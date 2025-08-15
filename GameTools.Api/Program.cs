var builder = WebApplication.CreateBuilder(args);


// dotnet-ef 설치
// dotnet tool install --global dotnet-ef

// 마이그레이션
// dotnet ef migrations add 마이그레이션이름 -p GameTools.Infrastructure/GameTools.Infrastructure.csproj -s GameTools.Api/GameTools.Api.csproj

// DB에 반영
// dotnet ef database update -p GameTools.Infrastructure/GameTools.Infrastructure.csproj -s GameTools.Api/GameTools.Api.csproj
