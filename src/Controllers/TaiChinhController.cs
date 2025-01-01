using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Manage_CLB_HTSV.Data;
using Manage_CLB_HTSV.Models;
using Manage_CLB_HTSV.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            var query = _context.TaiChinh.AsQueryable();

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

            var thongKeModel = new TaiChinhThongKeViewModel
            {
                TongThu = tongThu,
                TongChi = tongChi,
                TongSoGiaoDich = tongSoGiaoDich,
                TongTienGiaoDich = tongTienGiaoDich,
                LoaiGiaoDich = loaiGiaoDich,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                Labels = new List<string> { "Thu", "Chi" },
                Data = new List<decimal> { tongThu, tongChi }
            };

            return View(thongKeModel);
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
            var studentExists = _context.SinhVien.Any(s => s.MaSV == taiChinh.MaSV);
            if (!studentExists)
            {
                // Thêm lỗi vào ModelState nếu mã sinh viên không tồn tại
                ModelState.AddModelError("MaSV", "Mã sinh viên không tồn tại.");
            }
            // Thêm giao dịch vào cơ sở dữ liệu
            _context.Add(taiChinh);
            await _context.SaveChangesAsync();

            // Điều hướng đến trang Index sau khi thêm thành công
            return RedirectToAction(nameof(Index));
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
