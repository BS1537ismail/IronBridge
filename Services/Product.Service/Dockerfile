FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["../../Shared/IronBridge.Shared/IronBridge.Shared.csproj", "Shared/IronBridge.Shared/"]

# Copy service project
COPY ["Product.Service.csproj", "Product.Service/"]

# Restore dependencies
RUN dotnet restore "Product.Service/Product.Service.csproj"

# Copy everything else
COPY . Product.Service/
WORKDIR "/src/Product.Service"
RUN dotnet build "Product.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Product.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Service.dll"]
