using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Threading.Tasks;
using System.Linq;
using Manage_CLB_HTSV.Data;
using Microsoft.EntityFrameworkCore;
using Manage_CLB_HTSV.Models;

namespace Manage_CLB_HTSV.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, IEmailSender emailSender, ApplicationDbContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Tổng số sinh viên
            ViewBag.TotalStudents = await _context.SinhVien.CountAsync();

            // Tổng số hoạt động
            ViewBag.TotalActivities = await _context.HoatDong.CountAsync();

            // Tổng số tin tức
            ViewBag.TotalNews = await _context.TinTuc.CountAsync();

            // Tổng thu
            ViewBag.TotalIncome = await _context.TaiChinh.Where(t => t.LoaiGiaoDich == "Thu" == true).SumAsync(t => t.SoTien);

            // Thống kê hoạt động theo tháng
            var now = DateTime.Now;
            var sixMonthsAgo = now.AddMonths(-6);
            var activityStats = await _context.HoatDong
                .Where(h => h.ThoiGian >= sixMonthsAgo)
                .GroupBy(h => new { h.ThoiGian.Year, h.ThoiGian.Month })
                .Select(g => new { 
                    Month = g.Key.Year.ToString() + "/" + g.Key.Month.ToString(),
                    Count = g.Count() 
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            ViewBag.ActivityMonths = activityStats.Select(x => x.Month).ToList();
            ViewBag.ActivityCounts = activityStats.Select(x => x.Count).ToList();

            // Phân bố sinh viên theo khoa
            var facultyStats = await _context.SinhVien
                .Include(s => s.LopHoc)
                .ThenInclude(l => l.Khoa)
                .GroupBy(s => s.LopHoc.Khoa.TenKhoa)
                .Select(g => new {
                    FacultyName = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.FacultyNames = facultyStats.Select(x => x.FacultyName).ToList();
            ViewBag.FacultyStudentCounts = facultyStats.Select(x => x.Count).ToList();

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User with this email does not exist.");
                return View();
            }

            var newPassword = GenerateRandomPassword();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                await _emailSender.SendEmailAsync(email, "Reset Password", $"Your new password is: {newPassword}");
                return RedirectToAction("ResetPasswordConfirmation");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
        }

        private string GenerateRandomPassword()
        {
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random randNum = new Random();
            char[] chars = new char[8];
            for (int i = 0; i < 8; i++)
            {
                chars[i] = allowedChars[(int)(randNum.NextDouble() * allowedChars.Length)];
            }
            return new string(chars);
        }
    }
}
