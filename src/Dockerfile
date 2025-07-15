# ==============================================
# Stage 1: Base runtime image
# ==============================================
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

# Expose port for HTTP only (HTTPS via Cloudflare Tunnel)
EXPOSE 80

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser

# ==============================================
# Stage 2: Build environment
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files first for better caching
COPY ["Manage_CLB_HTSV.csproj", "./"]
COPY ["Manage-CLB-HTSV.generated.sln", "./"]

# Restore packages (this layer will be cached if project files don't change)
RUN dotnet restore "Manage_CLB_HTSV.csproj"

# Copy source code
COPY . .

# Build application
RUN dotnet build "Manage_CLB_HTSV.csproj" -c Release -o /app/build --no-restore

# ==============================================
# Stage 3: Publish
# ==============================================
FROM build AS publish

# Declare ARG variables for build-time secrets injection
ARG DB_PASSWORD2
ARG SMTP_PASSWORD
ARG MICROSOFT_CLIENT_ID
ARG MICROSOFT_CLIENT_SECRET

# Replace secrets in appsettings.json safely (work on the source)
RUN sed -i "s/\${secrets.DB_PASSWORD2}/${DB_PASSWORD2}/g" appsettings.json && \
    sed -i "s/\${secrets.SMTP_PASSWORD}/${SMTP_PASSWORD}/g" appsettings.json && \
    sed -i "s/\${secrets.MICROSOFT_CLIENT_ID}/${MICROSOFT_CLIENT_ID}/g" appsettings.json && \
    sed -i "s/\${secrets.MICROSOFT_CLIENT_SECRET}/${MICROSOFT_CLIENT_SECRET}/g" appsettings.json

# Publish application (will use existing build artifacts)
RUN dotnet publish "Manage_CLB_HTSV.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==============================================
# Stage 4: Final runtime image
# ==============================================
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create required directories for file uploads
RUN mkdir -p /app/wwwroot/qrcode && \
    mkdir -p /app/wwwroot/userimages && \
    mkdir -p /app/wwwroot/newsimages && \
    mkdir -p /app/wwwroot/minhchunghd && \
    mkdir -p /app/wwwroot/messages

# Note: HTTPS will be handled by Cloudflare Tunnel
# No certificates needed in container

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Add health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Manage_CLB_HTSV.dll"]
