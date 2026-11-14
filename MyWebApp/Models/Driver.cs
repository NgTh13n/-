using System.ComponentModel.DataAnnotations;
using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

/// <summary>
/// Hồ sơ Lái xe chuyên nghiệp của đơn vị (liên kết với Cán bộ)
/// </summary>
public class Driver
{
    public int Id { get; set; }

    // Liên kết với hồ sơ cán bộ (Officer)
    public int OfficerId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên lái xe.")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    public string FullName { get; set; } = "";

    public string Phone { get; set; } = "";

    // Số giấy phép lái xe mô phỏng
    public string LicenseNumberDemo { get; set; } = "";

    // Hạng bằng lái: B1, B2, C, D, E...
    [Required(ErrorMessage = "Vui lòng nhập hạng giấy phép lái xe.")]
    public string LicenseCategory { get; set; } = "B2";

    // Thuộc tính tương thích ngược với code cũ
    public string LicenseType
    {
        get => LicenseCategory;
        set => LicenseCategory = value;
    }

    // Thời hạn hiệu lực của giấy phép lái xe (dùng để kiểm tra khi điều phối)
    public DateTime LicenseExpiryDate { get; set; } = DateTime.Today.AddYears(3);

    public bool IsActive { get; set; } = true;

    public DriverStatus Status { get; set; } = DriverStatus.Active;

    public string Notes { get; set; } = "";
}
