using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Manage_CLB_HTSV.Data;
using Manage_CLB_HTSV.Models;
using Manage_CLB_HTSV.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClosedXML.Excel; // Add this namespace

namespace Manage_CLB_HTSV.Controllers
{
    [Authorize]
    public class TaiChinhController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiChinhController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ThongKe(string loaiGiaoDich, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.TaiChinh.Include(tc => tc.SinhVien).AsQueryable();

            if (!string.IsNullOrEmpty(loaiGiaoDich))
                query = query.Where(tc => tc.LoaiGiaoDich == loaiGiaoDich);

            if (tuNgay.HasValue)
                query = query.Where(tc => tc.NgayGiaoDich >= tuNgay.Value);

            if (denNgay.HasValue)
                query = query.Where(tc => tc.NgayGiaoDich <= denNgay.Value);

            var tongThu = await query.Where(tc => tc.LoaiGiaoDich == "Thu").SumAsync(tc => tc.SoTien);
            var tongChi = await query.Where(tc => tc.LoaiGiaoDich == "Chi").SumAsync(tc => tc.SoTien);
            var tongSoGiaoDich = await query.CountAsync();
            var tongTienGiaoDich = await query.SumAsync(tc => tc.SoTien);

            var danhSachGiaoDich = await query
                .OrderByDescending(tc => tc.NgayGiaoDich)
                .Select(tc => new TaiChinhListItemViewModel
                {
                    Id = tc.Id,
                    NgayGiaoDich = tc.NgayGiaoDich,
                    LoaiGiaoDich = tc.LoaiGiaoDich,
                    SoTien = tc.SoTien,
                    GhiChu = tc.GhiChu,
                    TenSinhVien = tc.SinhVien.HoTen
                })
                .ToListAsync();

            var thongKeModel = new TaiChinhThongKeViewModel
            {
                TongThu = tongThu,
                TongChi = tongChi,
                TongSoGiaoDich = tongSoGiaoDich,
                TongTienGiaoDich = tongTienGiaoDich,
                LoaiGiaoDich = loaiGiaoDich,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                DanhSachGiaoDich = danhSachGiaoDich
            };

            thongKeModel.Labels.AddRange(new[] { "Thu", "Chi" });
            thongKeModel.Data.AddRange(new[] { tongThu, tongChi });

            return View(thongKeModel);
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel(string loaiGiaoDich, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.TaiChinh.Include(tc => tc.SinhVien).AsQueryable();

            if (!string.IsNullOrEmpty(loaiGiaoDich))
                query = query.Where(tc => tc.LoaiGiaoDich == loaiGiaoDich);

            if (tuNgay.HasValue)
                query = query.Where(tc => tc.NgayGiaoDich >= tuNgay.Value);

            if (denNgay.HasValue)
                query = query.Where(tc => tc.NgayGiaoDich <= denNgay.Value);

            var data = await query
                .OrderByDescending(tc => tc.NgayGiaoDich)
                .Select(tc => new
                {
                    NgayGiaoDich = tc.NgayGiaoDich.ToString("dd/MM/yyyy"),
                    LoaiGiaoDich = tc.LoaiGiaoDich,
                    SoTien = tc.SoTien,
                    GhiChu = tc.GhiChu,
                    TenSinhVien = tc.SinhVien.HoTen
                })
                .ToListAsync();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Thống Kê Tài Chính");

            // Add headers
            worksheet.Cell(1, 1).Value = "Ngày Giao Dịch";
            worksheet.Cell(1, 2).Value = "Loại Giao Dịch";
            worksheet.Cell(1, 3).Value = "Số Tiền";
            worksheet.Cell(1, 4).Value = "Ghi Chú";
            worksheet.Cell(1, 5).Value = "Sinh Viên";

            // Add data
            var row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.NgayGiaoDich;
                worksheet.Cell(row, 2).Value = item.LoaiGiaoDich;
                worksheet.Cell(row, 3).Value = item.SoTien;
                worksheet.Cell(row, 4).Value = item.GhiChu;
                worksheet.Cell(row, 5).Value = item.TenSinhVien;
                row++;
            }

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Convert to bytes
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"ThongKeTaiChinh_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: TaiChinh
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.TaiChinh
                .Include(tc => tc.SinhVien)
                .ToListAsync();
            return View(transactions);
        }

        // GET: TaiChinh/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.TaiChinh
                .Include(tc => tc.SinhVien)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: TaiChinh/Create
        public IActionResult Create()
        {
            ViewBag.Students = _context.SinhVien.ToList();
            return View();
        }

       // POST: TaiChinh/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaSV,NgayGiaoDich,SoTien,LoaiGiaoDich,GhiChu")] TaiChinh taiChinh)
        {
            // Kiểm tra xem mã sinh viên có tồn tại không
            var studentExists = await _context.SinhVien.AnyAsync(s => s.MaSV == taiChinh.MaSV);
            if (!studentExists)
            {
                ModelState.AddModelError("MaSV", "Mã sinh viên không tồn tại trong hệ thống!");
                ViewBag.Students = await _context.SinhVien.ToListAsync();
                return View(taiChinh);
            }

                _context.Add(taiChinh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewBag.Students = await _context.SinhVien.ToListAsync();
            return View(taiChinh);
        }

        // GET: TaiChinh/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.TaiChinh.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewBag.Students = _context.SinhVien.ToList();
            return View(transaction);
        }

        // POST: TaiChinh/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaSV,NgayGiaoDich,SoTien,LoaiGiaoDich,GhiChu")] TaiChinh taiChinh)
        {
            if (id != taiChinh.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiChinh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiChinhExists(taiChinh.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Students = _context.SinhVien.ToList();
            return View(taiChinh);
        }

        // GET: TaiChinh/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.TaiChinh
                .Include(tc => tc.SinhVien)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: TaiChinh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.TaiChinh.FindAsync(id);
            if (transaction != null)
            {
                _context.TaiChinh.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaiChinhExists(int id)
        {
            return _context.TaiChinh.Any(e => e.Id == id);
        }
    }
}
