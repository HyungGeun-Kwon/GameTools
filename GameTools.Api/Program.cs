var builder = WebApplication.CreateBuilder(args);


// dotnet-ef ��ġ
// dotnet tool install --global dotnet-ef

// ���̱׷��̼�
// dotnet ef migrations add ���̱׷��̼��̸� -p GameTools.Infrastructure/GameTools.Infrastructure.csproj -s GameTools.Api/GameTools.Api.csproj

// DB�� �ݿ�
// dotnet ef database update -p GameTools.Infrastructure/GameTools.Infrastructure.csproj -s GameTools.Api/GameTools.Api.csproj
