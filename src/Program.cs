using Manage_CLB_HTSV;
using Manage_CLB_HTSV.Data;
using Manage_CLB_HTSV.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//Dang ky dich vu gui mail
builder.Services.AddSingleton<Manage_CLB_HTSV.IEmailSender, EmailSender>();

//Add signalr
builder.Services.AddSignalR();

//Set timeout cho phien dang nhap
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    
    // Xóa phần redirect cũ
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            // Chỉ redirect khi user thực sự cần đăng nhập
            if (context.Request.Path.StartsWithSegments("/Identity/Account/Login"))
            {
                return Task.CompletedTask;
            }
            
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Response.Redirect(options.LoginPath);
            }
            return Task.CompletedTask;
        }
    };
});

// Sửa lại cấu hình session
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".HTSV.Session";  // Đặt tên riêng cho session cookie
});

// Thêm cấu hình cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false; // Không yêu cầu consent cho cookies
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// Thiết lập giấy phép cho EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Thêm cấu hình xác thực bằng Microsoft Account
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    });

// Thêm cấu hình Kestrel từ appsettings.json
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(listenOptions =>
    {
        listenOptions.AllowAnyClientCertificate();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseExceptionHandler("/Home/Error");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCookiePolicy();
app.UseSession();
// app.UseMiddleware<SessionTimeoutMiddleware>(); // Tạm comment lại middleware này

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );

    // Route cho action Search
    endpoints.MapControllerRoute(
        name: "newsSearch",
        pattern: "News/Search",
        defaults: new { controller = "TinTucs", action = "Search" }
    );
    // Route cho action Search
    endpoints.MapControllerRoute(
        name: "sinhVienSearch",
        pattern: "SinhViens/Search",
        defaults: new { controller = "SinhViens", action = "Search" }
    );
    endpoints.MapControllerRoute(
       name: "downloadMinhChung",
       pattern: "ThamGiaHoatDongs/DownloadMinhChung/{filePath}",
       defaults: new { controller = "ThamGiaHoatDongs", action = "DownloadMinhChung" }
    );
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.MapRazorPages();

app.Run();