
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manage_CLB_HTSV.Models.ViewModel
{
    public class HoatDongViewModel
    {
        public IEnumerable<SelectListItem> HoatDongList { get; set; }
        public int SoLuongThamGia { get; set; }
        public int SoLuongVang { get; set; }
        public string TenHoatDong { get; set; }
        public IEnumerable<SinhVienThamGia> DanhSachSinhVien { get; set; }
        public string SelectedHoatDongId { get; set; } // Thêm trường này để lưu ID của hoạt động đã chọn
    }

}
