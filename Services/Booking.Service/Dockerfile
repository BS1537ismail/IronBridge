FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared project
COPY ["../../Shared/IronBridge.Shared/IronBridge.Shared.csproj", "Shared/IronBridge.Shared/"]

# Copy service project
COPY ["Booking.Service.csproj", "Booking.Service/"]

# Restore dependencies
RUN dotnet restore "Booking.Service/Booking.Service.csproj"

# Copy everything else
COPY . Booking.Service/
WORKDIR "/src/Booking.Service"
RUN dotnet build "Booking.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Booking.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.Service.dll"]
