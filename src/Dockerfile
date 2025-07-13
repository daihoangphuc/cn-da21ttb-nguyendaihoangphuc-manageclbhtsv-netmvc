# ==============================================
# Stage 1: Base runtime image
# ==============================================
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

# Expose ports for internal container communication
EXPOSE 80
EXPOSE 443

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
ARG PFX_PASSWORD
ARG MICROSOFT_CLIENT_ID
ARG MICROSOFT_CLIENT_SECRET

# Create a temporary appsettings for publish
RUN cp appsettings.json appsettings.json.template

# Replace secrets in appsettings.json safely
RUN sed -i "s/\${secrets.DB_PASSWORD2}/${DB_PASSWORD2}/g" appsettings.json && \
    sed -i "s/\${secrets.SMTP_PASSWORD}/${SMTP_PASSWORD}/g" appsettings.json && \
    sed -i "s/\${secrets.PFX_PASSWORD}/${PFX_PASSWORD}/g" appsettings.json && \
    sed -i "s/\${secrets.MICROSOFT_CLIENT_ID}/${MICROSOFT_CLIENT_ID}/g" appsettings.json && \
    sed -i "s/\${secrets.MICROSOFT_CLIENT_SECRET}/${MICROSOFT_CLIENT_SECRET}/g" appsettings.json

# Publish application
RUN dotnet publish "Manage_CLB_HTSV.csproj" -c Release -o /app/publish --no-restore --no-build

# ==============================================
# Stage 4: Final runtime image
# ==============================================
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Copy certificates if they exist (optional)
COPY ["Certificates/certificate.crt", "Certificates/private.key", "./"]
RUN if [ -f certificate.crt ] && [ -f private.key ]; then \
        openssl pkcs12 -export -out certificate.pfx -inkey private.key -in certificate.crt -password pass:"" && \
        chmod 600 private.key certificate.pfx && \
        chmod 644 certificate.crt && \
        chown appuser:appuser certificate.* ; \
    fi

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Add health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Manage_CLB_HTSV.dll"]
