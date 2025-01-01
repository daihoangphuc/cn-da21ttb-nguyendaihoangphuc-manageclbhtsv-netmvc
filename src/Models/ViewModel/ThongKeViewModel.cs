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
    }

}
