﻿using Manage_CLB_HTSV;
using Manage_CLB_HTSV.Data;
using Manage_CLB_HTSV.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Net;

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
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
});

// Add session services
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian sống của Session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Đánh dấu cookie session là thiết yếu cho ứng dụng
});

// Thiết lập giấy phép cho EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Thêm cấu hình xác thực bằng Microsoft Account
/*builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    });*/

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "AspNetCore.Identity.Application";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
/*
WebHost.CreateDefaultBuilder(args)
       .UseStartup<Program>()
       .UseKestrel(options =>
       {
           options.Listen(IPAddress.Any, 443, listenOptions =>
           {
               listenOptions.UseHttps("your_certificate.crt", "your_private_key.key");
           });
       });*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseSession();
app.UseMiddleware<SessionTimeoutMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Route cho action Search
    endpoints.MapControllerRoute(
        name: "newsSearch",
        pattern: "News/Search",
        defaults: new { controller = "TinTucs", action = "Search" });
    // Route cho action Search
    endpoints.MapControllerRoute(
        name: "sinhVienSearch",
        pattern: "SinhViens/Search",
        defaults: new { controller = "SinhViens", action = "Search" });
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.MapRazorPages();

app.Run();