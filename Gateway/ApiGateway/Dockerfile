FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy gateway project
COPY ["Gateway/ApiGateway/ApiGateway.csproj", "Gateway/ApiGateway/"]

# Restore dependencies
RUN dotnet restore "Gateway/ApiGateway/ApiGateway.csproj"

# Copy gateway source
COPY ["Gateway/ApiGateway/", "Gateway/ApiGateway/"]

WORKDIR "/src/Gateway/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
