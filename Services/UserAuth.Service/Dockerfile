FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["Shared/IronBridge.Shared/IronBridge.Shared.csproj", "Shared/IronBridge.Shared/"]

# Copy domain and repository projects
COPY ["Services/UserAuth.Repository/UserAuth.Repository.csproj", "Services/UserAuth.Repository/"]
COPY ["Services/UserAuth.Domain/UserAuth.Domain.csproj", "Services/UserAuth.Domain/"]

# Copy service project
COPY ["Services/UserAuth.Service/UserAuth.Service.csproj", "Services/UserAuth.Service/"]

# Restore dependencies
RUN dotnet restore "Services/UserAuth.Service/UserAuth.Service.csproj"

# Copy shared source
COPY ["Shared/IronBridge.Shared/", "Shared/IronBridge.Shared/"]

# Copy domain and repository source
COPY ["Services/UserAuth.Repository/", "Services/UserAuth.Repository/"]
COPY ["Services/UserAuth.Domain/", "Services/UserAuth.Domain/"]

# Copy service source
COPY ["Services/UserAuth.Service/", "Services/UserAuth.Service/"]

WORKDIR "/src/Services/UserAuth.Service"
RUN dotnet build "UserAuth.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserAuth.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserAuth.Service.dll"]
