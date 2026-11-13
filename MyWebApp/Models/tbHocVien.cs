using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Services.Models.CanBo;

public partial class tbHocVien
{
    [Key]

    [Display(Name = "Mã cán bộ")]
    [Description("Mã cán bộ")]
    public int IdCanBo { get; set; }


    [Display(Name = "Số hiệu CAND")]
    [Description("Số hiệu CAND")]
    [Placeholder("Ví dụ: 500-520")]
    public string? SoHieuCAND { get; set; }

    [Display(Name = "Giới tính")]
    [Description("Giới tính")]
    [Placeholder("Ví dụ: Nam/ Nữ")]
    public string? GioiTinh { get; set; }

    [Placeholder("Ví dụ: Nguyễn Văn A")]
    public string? HoTen { get; set; }
    public string? TenGoiKhac { get; set; }
    public DateTime? NgaySinh { get; set; }

    [Placeholder("Ví dụ: Phường Hoàn Kiếm, TP Hà Nội")]
    public string? NoiSinh { get; set; }
    public DateTime? NgayVaoDang { get; set; }
    public DateTime? NgayChinhThuc { get; set; }
    public string? DonViCT { get; set; }
    public string? Lop { get; set; }
    public string? DiaChiHienTai { get; set; }
}
