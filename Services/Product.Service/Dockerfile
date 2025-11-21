FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["Shared/IronBridge.Shared/IronBridge.Shared.csproj", "Shared/IronBridge.Shared/"]

# Copy domain and repository projects
COPY ["Services/Product.Repository/Product.Repository.csproj", "Services/Product.Repository/"]
COPY ["Services/Product.Domain/Product.Domain.csproj", "Services/Product.Domain/"]

# Copy service project
COPY ["Services/Product.Service/Product.Service.csproj", "Services/Product.Service/"]

# Restore dependencies
RUN dotnet restore "Services/Product.Service/Product.Service.csproj"

# Copy shared source
COPY ["Shared/IronBridge.Shared/", "Shared/IronBridge.Shared/"]

# Copy domain and repository source
COPY ["Services/Product.Repository/", "Services/Product.Repository/"]
COPY ["Services/Product.Domain/", "Services/Product.Domain/"]

# Copy service source
COPY ["Services/Product.Service/", "Services/Product.Service/"]

WORKDIR "/src/Services/Product.Service"
RUN dotnet build "Product.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Product.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Service.dll"]
