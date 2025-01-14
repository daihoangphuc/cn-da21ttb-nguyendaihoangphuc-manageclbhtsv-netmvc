using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manage_CLB_HTSV.Models.ViewModel
{
    public class TaiChinhThongKeViewModel
    {
        public decimal TongThu { get; set; }
        public decimal TongChi { get; set; }
        public int TongSoGiaoDich { get; set; }
        public decimal TongTienGiaoDich { get; set; }
        public string? LoaiGiaoDich { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public decimal SoDu => TongThu - TongChi;

        // Dữ liệu cho biểu đồ
        public List<string> Labels { get; set; } = new(); // Nhãn (ví dụ: "Thu", "Chi")
        public List<decimal> Data { get; set; } = new(); // Dữ liệu (ví dụ: giá trị tiền)

        // Danh sách giao dịch
        public List<TaiChinhListItemViewModel> DanhSachGiaoDich { get; set; } = new();
    }

    public class TaiChinhListItemViewModel
    {
        public int Id { get; set; }
        public DateTime NgayGiaoDich { get; set; }
        public string LoaiGiaoDich { get; set; }
        public decimal SoTien { get; set; }
        public string GhiChu { get; set; }
        public string TenSinhVien { get; set; }
    }

    public class HoatDongThongKeViewModel
    {
        public int TongSoHoatDong { get; set; }
        public int TongSoSinhVien { get; set; }
        public int TongSoLuotThamGia { get; set; }
        public int HoatDongHocKyHienTai { get; set; }
        public List<ThongKeHoatDongViewModel> TopHoatDong { get; set; } = new();
        public List<ThongKeTheoThangViewModel> ThongKeTheoThang { get; set; } = new();
    }

    public class ThongKeHoatDongViewModel
    {
        public string? TenHoatDong { get; set; }
        public int SoLuotThamGia { get; set; }
    }

    public class ThongKeTheoThangViewModel
    {
        public int Thang { get; set; }
        public int SoHoatDong { get; set; }
    }

    public class ThongKeSinhVienViewModel
    {
        public string? MaSV { get; set; }
        public string? HoTen { get; set; }
        public int SoHoatDongThamGia { get; set; }
    }

    public class ThongKeHoatDongHocKyViewModel
    {
        public int NamHoc { get; set; }
        public int HocKy { get; set; }
        public int SoLuongHoatDong { get; set; }
        public int SoLuotThamGia { get; set; }
    }
}
