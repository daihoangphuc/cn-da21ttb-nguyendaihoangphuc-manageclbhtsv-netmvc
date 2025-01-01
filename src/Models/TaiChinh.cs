using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Manage_CLB_HTSV.Models;

namespace Manage_CLB_HTSV.Models
{
    public class TaiChinh
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Khóa chính tự tăng

        [Required]
        [StringLength(20)]
        [DisplayName("Mã Sinh Viên")]
        public string? MaSV { get; set; } // Khóa ngoại từ bảng SinhVien

        [ForeignKey("MaSV")]
        public SinhVien? SinhVien { get; set; } // Điều hướng đến SinhVien

        [Required]
        [DisplayName("Ngày Giao Dịch")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime NgayGiaoDich { get; set; } // Ngày thực hiện giao dịch

        [Required]
        [DisplayName("Số Tiền")]
        public decimal SoTien { get; set; } // Số tiền giao dịch

        [Required]
        [StringLength(50)]
        [DisplayName("Loại Giao Dịch")]
        public string? LoaiGiaoDich { get; set; } // Loại giao dịch: "Thu" hoặc "Chi"

        [StringLength(255)]
        [DisplayName("Ghi Chú")]
        public string? GhiChu { get; set; } // Mô tả thêm về giao dịch
    }
}
