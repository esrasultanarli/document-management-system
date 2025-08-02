# Multi-stage build for .NET 8.0 application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["DocumentManagementSystem.csproj", "./"]
RUN dotnet restore "DocumentManagementSystem.csproj"

# Copy all source code
COPY . .
WORKDIR "/src"

# Build the application
RUN dotnet build "DocumentManagementSystem.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "DocumentManagementSystem.csproj" -c Release -o /app/publish

# Runtime stage
FROM base AS final
WORKDIR /app

# Create uploads directory
RUN mkdir -p /app/wwwroot/uploads

# Copy published application
COPY --from=publish /app/publish .

# Set permissions for uploads directory
RUN chmod 755 /app/wwwroot/uploads

# Debug: List all files
RUN ls -la
RUN find . -name "*.dll" -type f

ENTRYPOINT ["dotnet", "DocumentManagementSystem.dll"] 