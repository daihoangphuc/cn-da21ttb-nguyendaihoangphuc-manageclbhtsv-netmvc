# Hướng dẫn Setup Dự án

## Cấu hình appsettings.json

Để chạy dự án, bạn cần tạo file `appsettings.json` từ file mẫu:

1. Sao chép file `appsettings.json.example` thành `appsettings.json`
2. Thay thế các giá trị placeholder bằng thông tin thực tế của bạn:

### Database Connection
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=true"
}
```

### Email Settings
```json
"EmailSettings": {
  "MailServer": "smtp.gmail.com",
  "MailPort": 587,
  "Sender": "your-email@gmail.com",
  "Password": "your-app-password",
  "EnableSSL": true
}
```

### Microsoft Authentication
```json
"Authentication": {
  "Microsoft": {
    "ClientId": "your-microsoft-client-id",
    "ClientSecret": "your-microsoft-client-secret"
  }
}
```

### Admin Settings
```json
"AdminSettings": {
  "Email": "admin@example.com",
  "Password": "YourSecurePassword123!"
}
```

## Lưu ý bảo mật

- **KHÔNG BAO GIỜ** commit file `appsettings.json` chứa thông tin thực tế
- File `appsettings.json` đã được thêm vào `.gitignore`
- Chỉ commit file `appsettings.json.example` với placeholder values
- Sử dụng environment variables hoặc Azure Key Vault cho production

## Chạy dự án

1. Tạo và cấu hình file `appsettings.json`
2. Chạy migrations: `dotnet ef database update`
3. Chạy dự án: `dotnet run` 