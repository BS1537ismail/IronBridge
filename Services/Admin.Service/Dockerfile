FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["Shared/IronBridge.Shared/IronBridge.Shared.csproj", "Shared/IronBridge.Shared/"]

# Copy domain and repository projects
COPY ["Services/Admin.Repository/Admin.Repository.csproj", "Services/Admin.Repository/"]
COPY ["Services/Admin.Domain/Admin.Domain.csproj", "Services/Admin.Domain/"]

# Copy service project
COPY ["Services/Admin.Service/Admin.Service.csproj", "Services/Admin.Service/"]

# Restore dependencies
RUN dotnet restore "Services/Admin.Service/Admin.Service.csproj"

# Copy shared source
COPY ["Shared/IronBridge.Shared/", "Shared/IronBridge.Shared/"]

# Copy domain and repository source
COPY ["Services/Admin.Repository/", "Services/Admin.Repository/"]
COPY ["Services/Admin.Domain/", "Services/Admin.Domain/"]

# Copy service source
COPY ["Services/Admin.Service/", "Services/Admin.Service/"]

WORKDIR "/src/Services/Admin.Service"
RUN dotnet build "Admin.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Admin.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Admin.Service.dll"]
