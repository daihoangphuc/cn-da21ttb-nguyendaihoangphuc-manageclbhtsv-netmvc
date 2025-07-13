# 🚀 Deployment Guide - CLB HTSV Management System

## 📋 Overview

Hệ thống quản lý câu lạc bộ học tập sinh viên được containerize với Docker và triển khai tự động qua GitHub Actions.

## 🏗️ Architecture

- **Framework**: ASP.NET Core 6.0
- **Database**: SQL Server
- **Container**: Docker
- **CI/CD**: GitHub Actions
- **Ports**: 
  - **Local Development**: 80 (HTTP), 443 (HTTPS)
  - **Production**: 7516 (HTTP), 7517 (HTTPS)

## 🔧 Quick Start

### Development Environment

1. **Clone repository**
```bash
git clone <repository-url>
cd cn-da21ttb-nguyendaihoangphuc-manageclbhtsv-netmvc/src
```

2. **Set environment variables**
```bash
# Create .env file
cp .env.example .env

# Edit .env with your values
DB_PASSWORD2=your_db_password
SMTP_PASSWORD=your_smtp_password
MICROSOFT_CLIENT_ID=your_client_id
MICROSOFT_CLIENT_SECRET=your_client_secret
PFX_PASSWORD=your_certificate_password
```

3. **Run with Docker Compose (Local Development)**
```bash
docker-compose up -d
```

4. **Access application**
- HTTP: http://localhost
- HTTPS: https://localhost
- Health Check: http://localhost/health

### Production Deployment

Production deployment được tự động hóa qua GitHub Actions khi push vào branch `main`.

**📌 Lưu ý về Port Configuration:**
- **Local Development**: Chạy trên port 80/443 (tiêu chuẩn)
- **Production Host**: Chạy trên port 7516/7517 (custom)

```bash
# Local: 
http://localhost         # port 80
https://localhost        # port 443

# Production:
http://your-host:7516    # custom port cho production
https://your-host:7517   # custom port cho production
```

## 🚀 CI/CD Pipeline

### Pipeline Stages

1. **Code Quality & Testing**
   - Restore dependencies
   - Build application
   - Run unit tests (when available)

2. **Security Scanning**
   - Vulnerability scan with Trivy
   - Upload results to GitHub Security

3. **Build & Push**
   - Multi-stage Docker build
   - Push to Docker Hub
   - Docker image security scan

4. **Deploy to Production**
   - Blue-green deployment
   - Health checks
   - Automatic rollback on failure

5. **Post-deployment Verification**
   - Application health verification
   - Endpoint accessibility tests

### GitHub Secrets Required

```
DOCKER_USERNAME=your_dockerhub_username
DOCKER_PASSWORD=your_dockerhub_password
VPS_HOST=your_server_ip
VPS_USERNAME=your_server_username
SSH_PRIVATE_KEY=your_ssh_private_key
VPS_SSH_PORT=your_ssh_port
DB_PASSWORD2=your_database_password
SMTP_PASSWORD=your_email_password
PFX_PASSWORD=your_certificate_password
MICROSOFT_CLIENT_ID=your_microsoft_client_id
MICROSOFT_CLIENT_SECRET=your_microsoft_client_secret
```

## 🐳 Docker Commands

### Build Image
```bash
docker build -t webapp_htsv:latest \
  --build-arg DB_PASSWORD2="your_password" \
  --build-arg SMTP_PASSWORD="your_smtp" \
  --build-arg MICROSOFT_CLIENT_ID="your_client_id" \
  --build-arg MICROSOFT_CLIENT_SECRET="your_client_secret" \
  .
```

### Run Container
```bash
docker run -d --name webapp_htsv \
  -p 7516:80 \
  -p 7517:443 \
  --restart unless-stopped \
  --health-cmd "curl -f http://localhost:80/health || exit 1" \
  --health-interval=30s \
  --health-timeout=10s \
  --health-start-period=40s \
  --health-retries=3 \
  webapp_htsv:latest
```

### Monitor Logs
```bash
# View logs
docker logs webapp_htsv -f

# Check health status
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

## 🔍 Health Monitoring

### Health Check Endpoint
- **URL**: `/health`
- **Method**: GET
- **Response**: 200 OK if healthy

### Docker Health Check
Container includes built-in health check that:
- Runs every 30 seconds
- Times out after 10 seconds  
- Allows 40 seconds startup time
- Fails after 3 consecutive failures

## 🛠️ Troubleshooting

### Common Issues

1. **Container fails to start**
```bash
# Check logs
docker logs webapp_htsv

# Check if ports are available (local)
netstat -tulpn | grep :80

# Check if ports are available (production)
netstat -tulpn | grep :7516
```

2. **Health check fails**
```bash
# Test health endpoint manually (local)
curl -f http://localhost/health

# Test health endpoint manually (production)
curl -f http://localhost:7516/health

# Check database connection
docker exec webapp_htsv dotnet --version
```

3. **Certificate issues**
```bash
# Check certificate files in container
docker exec webapp_htsv ls -la /app/certificate.*
```

### Rollback Deployment

Automatic rollback occurs if health checks fail. Manual rollback:

```bash
# List available backup images
docker images | grep backup

# Stop current container
docker stop webapp_htsv
docker rm webapp_htsv

# Start backup
docker run -d --name webapp_htsv \
  -p 7516:80 -p 7517:443 \
  --restart unless-stopped \
  webapp_htsv:backup-YYYYMMDD_HHMMSS
```

## 📊 Performance Optimization

### Docker Build Optimization
- Multi-stage build reduces image size
- .dockerignore excludes unnecessary files
- Build cache layers optimize rebuild time

### Runtime Optimization
- Non-root user for security
- Health checks for reliability
- Resource limits (configure as needed)

## 🔐 Security Features

- Secrets injected at build time
- Non-root container user
- Vulnerability scanning in CI/CD
- SSL/TLS certificate support
- Regular security updates

## 📚 Additional Resources

- [ASP.NET Core Docker Documentation](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Make changes
4. Test locally with Docker
5. Submit pull request

## 📝 Notes

- Always test changes locally before pushing
- Monitor application logs after deployment
- Keep secrets updated and secure
- Regular backup of production data recommended 