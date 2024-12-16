# Step 1: Use ASP.NET 6.0 image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Step 2: Add certificate and private key files to the container
COPY ["Certificates/certificate.crt", "Certificates/private.key", "./"]

# Add this after copying certificates
RUN chmod 600 private.key && \
    chmod 644 certificate.crt && \
    # Verify files exist
    ls -la certificate.crt private.key

# Step 3: Set up HTTPS for Kestrel
ENV ASPNETCORE_URLS=http://+:80;https://+:443
RUN sed -i 's/TLSv1.2/TLSv1.0 TLSv1.1 TLSv1.2/g' /etc/ssl/openssl.cnf

# Step 5: Install the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .

# Declare ARG variables
ARG DB_PASSWORD2
ARG SMTP_PASSWORD
ARG PFX_PASSWORD
ARG MICROSOFT_CLIENT_ID
ARG MICROSOFT_CLIENT_SECRET

# Set environment variables
ENV DB_PASSWORD2=$DB_PASSWORD2
ENV SMTP_PASSWORD=$SMTP_PASSWORD
ENV PFX_PASSWORD=$PFX_PASSWORD
ENV MICROSOFT_CLIENT_ID=$MICROSOFT_CLIENT_ID
ENV MICROSOFT_CLIENT_SECRET=$MICROSOFT_CLIENT_SECRET

# Replace secrets in appsettings.json
RUN sed -i "s|\${secrets.DB_PASSWORD2}|$DB_PASSWORD2|g" appsettings.json
RUN sed -i "s|\${secrets.SMTP_PASSWORD}|$SMTP_PASSWORD|g" appsettings.json
RUN sed -i "s|\${secrets.PFX_PASSWORD}|$PFX_PASSWORD|g" appsettings.json
RUN sed -i "s|\${secrets.MICROSOFT_CLIENT_ID}|$MICROSOFT_CLIENT_ID|g" appsettings.json
RUN sed -i "s|\${secrets.MICROSOFT_CLIENT_SECRET}|$MICROSOFT_CLIENT_SECRET|g" appsettings.json

# Build application
RUN dotnet restore Manage-CLB-HTSV.generated.sln
RUN dotnet build Manage-CLB-HTSV.generated.sln -c Release -o /app/build

# Publish application
FROM build AS publish
RUN dotnet publish Manage-CLB-HTSV.generated.sln -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./

ENTRYPOINT ["dotnet", "Manage_CLB_HTSV.dll"]
