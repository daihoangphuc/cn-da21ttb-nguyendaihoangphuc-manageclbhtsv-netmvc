using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Manage_CLB_HTSV.Data;
using Manage_CLB_HTSV.Models;
using Manage_CLB_HTSV.Models.ViewModel;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Manage_CLB_HTSV.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
            // Đặt license context cho EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // GET: ThongKe
        public async Task<IActionResult> Index()
        {
            var viewModel = new HoatDongThongKeViewModel();

            // Thống kê tổng quan
            viewModel.TongSoHoatDong = await _context.HoatDong.CountAsync();
            viewModel.TongSoSinhVien = await _context.SinhVien.CountAsync();
            viewModel.TongSoLuotThamGia = await _context.ThamGiaHoatDong.CountAsync(t => t.DaThamGia);

            // Thống kê theo học kỳ hiện tại
            int namHoc = DateTime.Now.Year;
            int hocKy = DateTime.Now.Month <= 6 ? 2 : 1;

            viewModel.HoatDongHocKyHienTai = await _context.HoatDong
                .Where(h => h.NamHoc == namHoc && h.HocKy == hocKy)
                .CountAsync();

            // Top 5 hoạt động có nhiều sinh viên tham gia nhất
            viewModel.TopHoatDong = await _context.HoatDong
                .Select(h => new ThongKeHoatDongViewModel
                {
                    TenHoatDong = h.TenHoatDong,
                    SoLuotThamGia = _context.ThamGiaHoatDong
                        .Count(t => t.MaHoatDong == h.MaHoatDong && t.DaThamGia)
                })
                .OrderByDescending(x => x.SoLuotThamGia)
                .Take(5)
                .ToListAsync();

            // Thống kê theo tháng trong năm hiện tại
            viewModel.ThongKeTheoThang = await _context.HoatDong
                .Where(h => h.ThoiGian.Year == DateTime.Now.Year)
                .GroupBy(h => h.ThoiGian.Month)
                .Select(g => new ThongKeTheoThangViewModel
                {
                    Thang = g.Key,
                    SoHoatDong = g.Count()
                })
                .OrderBy(x => x.Thang)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: ThongKe/SinhVien
        public async Task<IActionResult> SinhVien()
        {
            var topSinhVien = await _context.SinhVien
                .Select(sv => new ThongKeSinhVienViewModel
                {
                    MaSV = sv.MaSV,
                    HoTen = sv.HoTen,
                    SoHoatDongThamGia = _context.ThamGiaHoatDong
                        .Count(t => t.MaSV == sv.MaSV && t.DaThamGia)
                })
                .OrderByDescending(x => x.SoHoatDongThamGia)
                .Take(10)
                .ToListAsync();

            return View(topSinhVien);
        }

        // GET: ThongKe/HoatDong
        public async Task<IActionResult> HoatDong(int? namHoc, int? hocKy)
        {
            var query = _context.HoatDong.AsQueryable();

            // Lấy danh sách năm học để đưa vào ViewBag
            ViewBag.NamHocList = await _context.HoatDong
                .Select(h => h.NamHoc)
                .Distinct()
                .OrderByDescending(n => n)
                .ToListAsync();

            // Nếu có lọc theo năm học
            if (namHoc.HasValue)
            {
                query = query.Where(h => h.NamHoc == namHoc.Value);
                ViewBag.SelectedNamHoc = namHoc.Value;
            }

            // Nếu có lọc theo học kỳ
            if (hocKy.HasValue)
            {
                query = query.Where(h => h.HocKy == hocKy.Value);
                ViewBag.SelectedHocKy = hocKy.Value;
            }

            var thongKeHoatDong = await query
                .GroupBy(h => new { h.NamHoc, h.HocKy })
                .Select(g => new ThongKeHoatDongHocKyViewModel
                {
                    NamHoc = g.Key.NamHoc,
                    HocKy = g.Key.HocKy,
                    SoLuongHoatDong = g.Count(),
                    SoLuotThamGia = _context.ThamGiaHoatDong
                        .Count(t => g.Select(h => h.MaHoatDong).Contains(t.MaHoatDong) && t.DaThamGia)
                })
                .OrderByDescending(x => x.NamHoc)
                .ThenByDescending(x => x.HocKy)
                .ToListAsync();

            return View(thongKeHoatDong);
        }

        // GET: ThongKe/XuatExcel
        public async Task<IActionResult> XuatExcel(int? namHoc, int? hocKy)
        {
            var query = _context.HoatDong.AsQueryable();

            if (namHoc.HasValue)
            {
                query = query.Where(h => h.NamHoc == namHoc.Value);
            }

            if (hocKy.HasValue)
            {
                query = query.Where(h => h.HocKy == hocKy.Value);
            }

            var thongKeHoatDong = await query
                .GroupBy(h => new { h.NamHoc, h.HocKy })
                .Select(g => new ThongKeHoatDongHocKyViewModel
                {
                    NamHoc = g.Key.NamHoc,
                    HocKy = g.Key.HocKy,
                    SoLuongHoatDong = g.Count(),
                    SoLuotThamGia = _context.ThamGiaHoatDong
                        .Count(t => g.Select(h => h.MaHoatDong).Contains(t.MaHoatDong) && t.DaThamGia)
                })
                .OrderByDescending(x => x.NamHoc)
                .ThenByDescending(x => x.HocKy)
                .ToListAsync();

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("ThongKeHoatDong");

                // Thiết lập header
                worksheet.Cells["A1"].Value = "THỐNG KÊ HOẠT ĐỘNG THEO HỌC KỲ";
                worksheet.Cells["A1:E1"].Merge = true;
                worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                worksheet.Cells["A1:E1"].Style.Font.Size = 16;
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Thiết lập tiêu đề cột
                worksheet.Cells["A3"].Value = "STT";
                worksheet.Cells["B3"].Value = "Năm học";
                worksheet.Cells["C3"].Value = "Học kỳ";
                worksheet.Cells["D3"].Value = "Số lượng hoạt động";
                worksheet.Cells["E3"].Value = "Số lượt tham gia";
                worksheet.Cells["F3"].Value = "Tỷ lệ tham gia";

                // Style cho tiêu đề cột
                var headerRange = worksheet.Cells["A3:F3"];
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Điền dữ liệu
                int row = 4;
                int stt = 1;
                foreach (var item in thongKeHoatDong)
                {
                    worksheet.Cells[row, 1].Value = stt++;
                    worksheet.Cells[row, 2].Value = item.NamHoc;
                    worksheet.Cells[row, 3].Value = item.HocKy;
                    worksheet.Cells[row, 4].Value = item.SoLuongHoatDong;
                    worksheet.Cells[row, 5].Value = item.SoLuotThamGia;
                    worksheet.Cells[row, 6].Formula = $"IF(D{row}=0,0,E{row}/D{row})";
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "0.00%";
                    row++;
                }

                // Tự động điều chỉnh độ rộng cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Style cho toàn bộ dữ liệu
                var dataRange = worksheet.Cells[3, 1, row - 1, 6];
                dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                await package.SaveAsync();
            }

            stream.Position = 0;
            string fileName = $"ThongKeHoatDong_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
