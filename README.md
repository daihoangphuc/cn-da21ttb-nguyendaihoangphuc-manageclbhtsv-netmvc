# Website Quản Lý Câu Lạc Bộ Hành Trình Sinh Viên

## Mô tả dự án
Website này được xây dựng để hỗ trợ quản lý các hoạt động của câu lạc bộ Hành trình Sinh viên. Với các tính năng quản lý thành viên, tổ chức và theo dõi hoạt động, hệ thống giúp tối ưu hóa quá trình quản lý và tăng cường tương tác giữa các thành viên trong câu lạc bộ.

---

## Mục tiêu dự án
- Xây dựng một hệ thống quản lý câu lạc bộ đáp ứng các yêu cầu thực tế, bao gồm quản lý thành viên, hoạt động, và các thông tin liên quan.
- Tìm hiểu và ứng dụng công nghệ **ASP.NET MVC 6** kết hợp với các công nghệ hỗ trợ như **HTML**, **CSS (Bootstrap)**, **JavaScript**, và **SQL Server**.
- Tạo một website thân thiện, dễ sử dụng và tương thích tốt trên cả thiết bị di động và máy tính.

---

## Tính năng chính

### Quản lý cho Quản trị viên (Admin)
- **Dashboard & Thống kê**: Giao diện tổng quan với các biểu đồ thống kê về số lượng sinh viên, hoạt động, và tài chính.
- **Quản lý Thành viên**:
    - Quản lý thông tin sinh viên, bao gồm thêm, sửa, xóa.
    - Phân quyền và quản lý vai trò người dùng (Admin, Member).
    - Quản lý Ban Chủ nhiệm câu lạc bộ.
- **Quản lý Hoạt động**:
    - Tạo, cập nhật, và xóa các hoạt động, sự kiện của CLB.
    - Theo dõi danh sách sinh viên đăng ký tham gia hoạt động.
    - Cập nhật minh chứng tham gia hoạt động cho sinh viên.
- **Quản lý Tài chính**:
    - Ghi nhận và quản lý các khoản thu, chi của câu lạc bộ.
    - Thống kê tài chính theo thời gian.
- **Quản lý Tin tức**: Đăng tải và quản lý các bài viết, thông báo trên trang chủ.
- **Quản lý Dữ liệu chung**: Quản lý danh mục Khoa, Lớp học, Chức vụ.

### Tính năng cho Thành viên (Sinh viên)
- **Trang chủ**: Xem các hoạt động và tin tức mới nhất từ câu lạc bộ.
- **Đăng ký hoạt động**: Xem danh sách các hoạt động sắp diễn ra và đăng ký tham gia trực tuyến.
- **Quản lý hoạt động đã đăng ký**: Xem lại các hoạt động đã đăng ký và hủy đăng ký nếu cần.
- **Cập nhật thông tin cá nhân**: Tự quản lý và cập nhật thông tin cá nhân.
- **Chat**: Giao tiếp, trao đổi trực tuyến với các thành viên khác.

---

## Công nghệ sử dụng
- **Backend**: ASP.NET Core MVC 6, C#
- **Frontend**: HTML, CSS (Bootstrap 5), JavaScript, jQuery
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core 6
- **Authentication**: ASP.NET Core Identity
- **Real-time**: SignalR (cho tính năng Chat)

---

## Hướng dẫn cài đặt và chạy dự án

### Yêu cầu
- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Một IDE như [Visual Studio 2022](https://visualstudio.microsoft.com/) hoặc [Visual Studio Code](https://code.visualstudio.com/).

### Các bước cài đặt

1.  **Clone repository:**
    ```bash
    git clone <your-repository-url>
    cd cn-da21ttb-nguyendaihoangphuc-manageclbhtsv-netmvc/src
    ```

2.  **Cấu hình Connection String:**
    - Sao chép file `src/appsettings.json.example` thành `src/appsettings.json`.
    - Cập nhật các thông tin cấu hình trong file `appsettings.json`:
      - Connection String cho database
      - Email settings
      - Microsoft Authentication (nếu sử dụng)
      - Admin settings (email và password cho tài khoản admin)
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=Manage_CLB_HTSV_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
    ```
    ⚠️ **Lưu ý bảo mật**: Không push file `appsettings.json` lên repository công khai!

3.  **Cập nhật Database:**
    - Mở terminal hoặc Command Prompt trong thư mục `src`.
    - Chạy lệnh sau để áp dụng các migrations và tạo cơ sở dữ liệu:
    ```bash
    dotnet ef database update
    ```

4.  **Cài đặt Chứng chỉ (Tùy chọn):**
    - Nếu bạn muốn chạy ứng dụng với HTTPS trên môi trường production, hãy đặt các tệp chứng chỉ vào thư mục `src/Certificates/` như mô tả bên dưới.
    - Đối với môi trường development, bạn có thể sử dụng chứng chỉ tự ký của .NET. Chạy lệnh sau để tin tưởng chứng chỉ:
    ```bash
    dotnet dev-certs https --trust
    ```

5.  **Chạy ứng dụng:**
    - Sử dụng terminal trong thư mục `src`:
    ```bash
    dotnet run
    ```
    - Hoặc chạy trực tiếp từ Visual Studio/VS Code.

6.  **Tài khoản Admin mặc định:**
    - Hệ thống sẽ tự động tạo tài khoản admin dựa trên cấu hình trong `appsettings.json`.
    - Cấu hình trong section `AdminSettings`:
      ```json
      "AdminSettings": {
        "Email": "admin@example.com",
        "Password": "YourSecurePassword123!"
      }
      ```
    - Hoặc sử dụng Environment Variables:
      - `ADMIN_EMAIL`: Email của admin
      - `ADMIN_PASSWORD`: Password của admin

---

## Cấu trúc thư mục dự án
```
src/
├── Controllers/      # Chứa các lớp Controller xử lý request
├── Models/           # Chứa các lớp Model (data entities)
├── Views/            # Chứa các file View (.cshtml) cho giao diện
├── Data/             # Chứa DbContext và các file Migrations
├── wwwroot/          # Chứa các tài sản tĩnh (CSS, JS, images)
├── Areas/Identity/   # Chứa các trang quản lý người dùng của ASP.NET Identity
├── ViewModels/       # Chứa các model dùng riêng cho View
├── Components/       # Chứa các View Components
├── Properties/       # Chứa file cấu hình launchSettings.json
├── Program.cs        # File khởi tạo và cấu hình ứng dụng
└── Manage_CLB_HTSV.csproj # File projet của .NET
```

---

## Cài đặt Chứng chỉ SSL/TLS

Để chạy ứng dụng với kết nối an