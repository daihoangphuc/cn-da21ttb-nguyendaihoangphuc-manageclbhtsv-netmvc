using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Manage_CLB_HTSV.Models;

namespace Manage_CLB_HTSV.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Các DbSet cho các thực thể của bạn
        public DbSet<Khoa> Khoa { get; set; }
        public DbSet<LopHoc> LopHoc { get; set; }
        public DbSet<ChucVu> ChucVu { get; set; }
        public DbSet<SinhVien> SinhVien { get; set; }
        public DbSet<HoatDong> HoatDong { get; set; }
        public DbSet<DangKyHoatDong> DangKyHoatDong { get; set; }
        public DbSet<TinTuc> TinTuc { get; set; }
        public DbSet<ThamGiaHoatDong> ThamGiaHoatDong { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Đổi tên bảng Identity
            modelBuilder.Entity<IdentityUser>(b =>
            {
                b.ToTable("Users"); // Đổi tên bảng AspNetUsers thành Users
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles"); // Đổi tên bảng AspNetRoles thành Roles
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles"); // Đổi tên bảng AspNetUserRoles thành UserRoles
            });


            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaim"); // Đổi tên bảng AspNetUserRoles thành UserRoles
            });


            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaim"); // Đổi tên bảng AspNetUserRoles thành UserRoles
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("UserLogin"); // Đổi tên bảng AspNetUserRoles thành UserRoles
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserToken"); // Đổi tên bảng AspNetUserRoles thành UserRoles
            });
        }
    }
}
