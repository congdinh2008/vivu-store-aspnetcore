FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
# EXPOSE 443  # Comment out HTTPS port for now

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ViVuStore.API/ViVuStore.API.csproj", "ViVuStore.API/"]
COPY ["ViVuStore.Business/ViVuStore.Business.csproj", "ViVuStore.Business/"]
COPY ["ViVuStore.Data/ViVuStore.Data.csproj", "ViVuStore.Data/"]
COPY ["ViVuStore.Core/ViVuStore.Core.csproj", "ViVuStore.Core/"]
COPY ["ViVuStore.Models/ViVuStore.Models.csproj", "ViVuStore.Models/"]
RUN dotnet restore "ViVuStore.API/ViVuStore.API.csproj"
COPY . .
WORKDIR "/src/ViVuStore.API"
RUN dotnet build "ViVuStore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ViVuStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for seed data
RUN mkdir -p /app/wwwroot/data

# Explicitly copy the seed data files from the source to the container
# Make sure these paths are correct
COPY ViVuStore.API/wwwroot/data/roles.json /app/wwwroot/data/
COPY ViVuStore.API/wwwroot/data/users.json /app/wwwroot/data/

# For debugging - list the contents of the directory
RUN ls -la /app/wwwroot/data/

# Set environment variables with defaults
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ConnectionStrings__ViVuStoreDbConnection="Server=db;Database=ViVuStoreDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
ENV ASPNETCORE_URLS=http://+:80
ENV JWT__Secret="Lg2eUhBHLbUSenLFZwjUlBn4q34TgJmjkwSzdtspci/nNYaUpd23REGxqT9khG+eAtQmnyYvIYrnRTTmVmn+rA=="
ENV JWT__Issuer="ViVuStore"
ENV JWT__Audience="ViVuStoreClient"
ENV JWT__AccessTokenExpiryMinutes="15"
ENV JWT__RefreshTokenExpiryDays="7"
ENV DB_PORT=1433

ENTRYPOINT ["dotnet", "ViVuStore.API.dll"]
