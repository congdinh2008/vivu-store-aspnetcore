# ViVuStore Web API

This project is an ASP.NET Core 9.0 Web API for ViVuStore, with Docker support for both development and production environments.

## Technologies Used

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- SQL Server 2022
- Docker & Docker Compose
- JWT Authentication
- Identity Framework
- MediatR
- AutoMapper
- Swagger / OpenAPI

## Prerequisites

- Docker Desktop
- Docker Compose
- .NET 9.0 SDK (for local development)
- Git

## Running with Docker Compose

The application is containerized and can be run using Docker Compose, which will set up:
- SQL Server database
- Web API application

### Quick Start

1. Clone the repository:
```bash
git clone <repository-url>
cd /path/to/ViVuStore
```

2. Build and start the containers:
```bash
docker-compose up -d
```

3. Access the API:
   - API: http://localhost:5176/swagger
   - API Documentation: http://localhost:5176/swagger/index.html

4. Stop the containers:
```bash
docker-compose down
```

### Docker Configuration

The project includes Docker configuration files:

1. Create your Docker settings file by copying the sample:
```bash
cp ViVuStore.API/appsettings.DockerSample.json ViVuStore.API/appsettings.Docker.json
```

2. Edit the Docker settings file to match your environment:
```bash
nano ViVuStore.API/appsettings.Docker.json
```

3. Sample Docker configuration (`appsettings.Docker.json`):
```json
{
  "ConnectionStrings": {
    "ViVuStoreDbConnection": "Server=db;Database=ViVuStoreDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "JWT": {
    "Secret": "YourSecretKeyHere_MakeItLongAndComplexForProduction",
    "Issuer": "ViVuStore",
    "Audience": "ViVuStoreClient",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "CORs": {
    "AllowedOrigins": "http://localhost:4200,https://localhost:4200",
    "AllowedMethods": "GET,POST,PUT,DELETE,OPTIONS",
    "AllowedHeaders": "Content-Type,Authorization"
  }
}
```

### Docker Compose Commands

- Build and start containers:
```bash
docker-compose up -d
```

- View logs:
```bash
docker-compose logs -f
```

- Stop containers:
```bash
docker-compose down
```

- Rebuild containers:
```bash
docker-compose build --no-cache
```

- Remove containers and volumes:
```bash
docker-compose down -v
```

## Application Structure

- **ViVuStore.API**: Web API project with controllers
- **ViVuStore.Business**: Business logic layer
- **ViVuStore.Data**: Data access layer with EF Core
- **ViVuStore.Core**: Core utilities and constants
- **ViVuStore.Models**: Domain models, DTOs and view models

## Database

The application uses SQL Server as the database. The connection string is configured in:

- Local development: `appsettings.json`
- Docker: `appsettings.Docker.json` and environment variables in `docker-compose.yml`

The database is automatically migrated and seeded with initial data when the application starts.

### Seed Data

The application includes seed data for:
- User roles (in `wwwroot/data/roles.json`)
- Users (in `wwwroot/data/users.json`)

## Authentication and Authorization

The API uses JWT tokens for authentication. To authenticate:

1. Use the `/api/Auth/login` endpoint with username and password
2. The response includes an access token and refresh token
3. Include the access token in subsequent requests in the Authorization header:
   `Authorization: Bearer <token>`

Default user credentials:

- Admin User:
  - Username: `systemadministrator`
  - Password: `Admin@1234`

- Regular User:
  - Username: `thangnguyen`
  - Password: `User@1234`

## API Documentation

API documentation is available through Swagger UI:
- Development: http://localhost:5176/swagger
- Docker: http://localhost:5176/swagger

## Environment Configuration

The application supports different environments:

- **Development**: Local development environment
- **Docker**: Docker environment
- **Production**: Production environment

Environment-specific settings are in corresponding `appsettings.<Environment>.json` files.

## Docker Configuration Files

- **Dockerfile**: Instructions to build the API container
- **docker-compose.yml**: Defines the services (API, SQL Server)
- **.dockerignore**: Files to exclude from the Docker build context
- **.env**: Environment variables for Docker Compose
- **appsettings.Docker.json**: Configuration for Docker environment
  - Copy from `appsettings.DockerSample.json` to get started
  - This file is in `.gitignore` to prevent committing sensitive data
  - Customize as needed for your environment

## Custom Docker Configuration

For advanced users, you can customize the Docker environment:

1. Modify environment variables in `docker-compose.yml`:
   ```yaml
   environment:
     - ASPNETCORE_ENVIRONMENT=Docker
     - ConnectionStrings__ViVuStoreDbConnection=Server=db;Database=YourDatabase;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
   ```

2. Create a custom `.env` file with your settings:
   ```
   ACCEPT_EULA=Y
   MSSQL_SA_PASSWORD=YourCustomPassword
   ```

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server container is running: `docker ps`
- Check logs: `docker-compose logs db`
- Verify connection string in environment variables

### API Container Issues
- Check container logs: `docker-compose logs api`
- Ensure database is healthy before API starts
- Verify environment variables are set correctly

### Missing Seed Data
- Verify files exist in container: `docker exec -it vivustore-api ls -la /app/wwwroot/data`
- Check for errors in container logs: `docker-compose logs api`

## License

[License information goes here]
