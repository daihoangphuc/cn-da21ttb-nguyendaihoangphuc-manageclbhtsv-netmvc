using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Manage_CLB_HTSV.Models;
using Manage_CLB_HTSV.Data;
using Manage_CLB_HTSV.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Manage_CLB_HTSV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                LatestNews = _context.TinTuc.OrderByDescending(t => t.NgayDang).Take(5).ToList(),
                UpcomingActivities = _context.HoatDong
                    .Where(h => h.ThoiGian > DateTime.Now)
                    .OrderBy(h => h.ThoiGian)
                    .Take(5)
                    .ToList()
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        /*       public IActionResult Error()
               {
                   return View(); // Trả về trang báo lỗi
               }*/
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}