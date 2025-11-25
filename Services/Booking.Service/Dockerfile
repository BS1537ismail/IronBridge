FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["Shared/IronBridge.Shared/IronBridge.Shared.csproj", "Shared/IronBridge.Shared/"]

# Copy domain and repository projects
COPY ["Services/Booking.Repository/Booking.Repository.csproj", "Services/Booking.Repository/"]
COPY ["Services/Booking.Domain/Booking.Domain.csproj", "Services/Booking.Domain/"]

# Copy service project
COPY ["Services/Booking.Service/Booking.Service.csproj", "Services/Booking.Service/"]

# Restore dependencies
RUN dotnet restore "Services/Booking.Service/Booking.Service.csproj"

# Copy shared source
COPY ["Shared/IronBridge.Shared/", "Shared/IronBridge.Shared/"]

# Copy domain and repository source
COPY ["Services/Booking.Repository/", "Services/Booking.Repository/"]
COPY ["Services/Booking.Domain/", "Services/Booking.Domain/"]

# Copy service source
COPY ["Services/Booking.Service/", "Services/Booking.Service/"]

WORKDIR "/src/Services/Booking.Service"
RUN dotnet build "Booking.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Booking.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.Service.dll"]
