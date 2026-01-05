# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EAD_project.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage - use alpine for smaller image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Create writable directories for SQLite database and Data Protection keys
RUN mkdir -p /app/data /app/keys && chmod 777 /app/data /app/keys

COPY --from=build /app/publish .

# Add volume for key persistence
VOLUME ["/app/keys"]

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_gcServer=0
ENV DOTNET_GCHeapHardLimit=100000000

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/Login || exit 1

ENTRYPOINT ["dotnet", "EAD_project.dll"]
