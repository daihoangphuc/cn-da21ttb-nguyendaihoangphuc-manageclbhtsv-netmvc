using Manage_CLB_HTSV.Data;
using Manage_CLB_HTSV.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Manage_CLB_HTSV.Components
{
    public class ThongKeThamGiaHD : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ThongKeThamGiaHD(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string selectedHoatDongId = null)
        {
            // Lấy danh sách hoạt động đã kết thúc
            var hoatDongList = await _context.HoatDong
                .Where(t => t.TrangThai == "Đã kết thúc")
                .Select(a => new SelectListItem
                {
                    Value = a.MaHoatDong.ToString(),
                    Text = a.TenHoatDong
                })
                .ToListAsync();

            // Khởi tạo ViewModel
            var viewModel = new HoatDongViewModel
            {
                HoatDongList = hoatDongList,
                SelectedHoatDongId = selectedHoatDongId
            };

            if (!string.IsNullOrEmpty(selectedHoatDongId))
            {
                // Nếu có hoạt động đã chọn, tính toán số lượng tham gia và vắng mặt
                var soLuongThamGia = await _context.ThamGiaHoatDong
                    .CountAsync(tg => tg.MaHoatDong == selectedHoatDongId && tg.DaThamGia == true);

                var soLuongVang = await _context.ThamGiaHoatDong
                    .CountAsync(tg => tg.MaHoatDong == selectedHoatDongId && tg.DaThamGia == false);

                var hoatDong = await _context.HoatDong.FindAsync(selectedHoatDongId);

                // Cập nhật số liệu vào ViewModel
                viewModel.SoLuongThamGia = soLuongThamGia;
                viewModel.SoLuongVang = soLuongVang;
                viewModel.TenHoatDong = hoatDong?.TenHoatDong;

                // Lấy danh sách sinh viên tham gia hoạt động
                var danhSachSinhVien = await _context.ThamGiaHoatDong
                    .Where(tg => tg.MaHoatDong == selectedHoatDongId && tg.DaThamGia == true)
                    .Include(t => t.DangKyHoatDong)
                    .Include(h => h.DangKyHoatDong.HoatDong)
                    .Include(s => s.DangKyHoatDong.SinhVien)
                    .Select(tg => new SinhVienThamGia
                    {
                        MaSV = tg.DangKyHoatDong.SinhVien.MaSV,
                        HoTen = tg.DangKyHoatDong.SinhVien.HoTen,
                        MaLop = tg.DangKyHoatDong.SinhVien.MaLop
                    })
                    .ToListAsync();

                viewModel.DanhSachSinhVien = danhSachSinhVien;
            }

            return View(viewModel); // Trả về View với ViewModel
        }

    }

}