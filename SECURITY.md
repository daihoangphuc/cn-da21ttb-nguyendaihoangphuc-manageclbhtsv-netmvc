# Hướng dẫn Bảo mật

## ⚠️ Cảnh báo Bảo mật

### Files không được push lên repository:
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Production.json`
- Files trong thư mục `Certificates/`
- Files trong thư mục `logs/`
- Files upload của user trong `wwwroot/userimages/`, `wwwroot/newsimages/`, etc.

### Thông tin nhạy cảm cần bảo vệ:
1. **Database Connection String**
2. **Email SMTP Password**
3. **Microsoft Authentication Keys**
4. **Admin Account Credentials**
5. **SSL Certificates**

## 🔧 Cấu hình Bảo mật

### 1. Cấu hình Local Development:
```bash
# Sao chép file template
cp src/appsettings.json.example src/appsettings.json

# Cập nhật thông tin thực tế vào appsettings.json
# KHÔNG commit file này lên git
```

### 2. Cấu hình Production với Environment Variables:
```bash
export ADMIN_EMAIL="your-admin@email.com"
export ADMIN_PASSWORD="YourSecurePassword123!"
export ConnectionStrings__DefaultConnection="Server=...;Database=...;User Id=...;Password=..."
```

### 3. Cấu hình Docker:
```dockerfile
ENV ADMIN_EMAIL=your-admin@email.com
ENV ADMIN_PASSWORD=YourSecurePassword123!
```

## 🛡️ Best Practices

1. **Sử dụng mật khẩu mạnh** cho tài khoản admin
2. **Không hardcode** thông tin nhạy cảm trong code
3. **Sử dụng Environment Variables** cho production
4. **Backup database** định kỳ
5. **Cập nhật dependencies** thường xuyên
6. **Sử dụng HTTPS** trong production
7. **Giới hạn quyền truy cập** database

## 🔐 Tạo mật khẩu an toàn

Mật khẩu admin nên:
- Tối thiểu 8 ký tự
- Có chữ hoa, chữ thường
- Có số và ký tự đặc biệt
- Không sử dụng thông tin cá nhân

Ví dụ: `MySecure2024!App` 