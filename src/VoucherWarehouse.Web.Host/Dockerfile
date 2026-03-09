FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/VoucherWarehouse.Web.Host/VoucherWarehouse.Web.Host.csproj", "src/VoucherWarehouse.Web.Host/"]
COPY ["src/VoucherWarehouse.Web.Core/VoucherWarehouse.Web.Core.csproj", "src/VoucherWarehouse.Web.Core/"]
COPY ["src/VoucherWarehouse.Application/VoucherWarehouse.Application.csproj", "src/VoucherWarehouse.Application/"]
COPY ["src/VoucherWarehouse.Core/VoucherWarehouse.Core.csproj", "src/VoucherWarehouse.Core/"]
COPY ["src/VoucherWarehouse.EntityFrameworkCore/VoucherWarehouse.EntityFrameworkCore.csproj", "src/VoucherWarehouse.EntityFrameworkCore/"]
WORKDIR "/src/src/VoucherWarehouse.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/VoucherWarehouse.Web.Host", "src/VoucherWarehouse.Web.Host"]
COPY ["src/VoucherWarehouse.Web.Core", "src/VoucherWarehouse.Web.Core"]
COPY ["src/VoucherWarehouse.Application", "src/VoucherWarehouse.Application"]
COPY ["src/VoucherWarehouse.Core", "src/VoucherWarehouse.Core"]
COPY ["src/VoucherWarehouse.EntityFrameworkCore", "src/VoucherWarehouse.EntityFrameworkCore"]
WORKDIR "/src/src/VoucherWarehouse.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "VoucherWarehouse.Web.Host.dll"]
