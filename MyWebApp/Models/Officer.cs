using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models;

/// <summary>
/// Hồ sơ Cán bộ / Chiến sĩ trong đơn vị Công an
/// </summary>
public class Officer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã cán bộ.")]
    [StringLength(20, ErrorMessage = "Mã cán bộ tối đa 20 ký tự.")]
    public string Code { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ tên cán bộ.")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    public string FullName { get; set; } = "";

    // Cấp bậc / Chức vụ CAND: Thiếu úy, Trung úy, Thượng úy, Đại úy, Thiếu tá, Trung tá, Thượng tá...
    public string RankOrTitle { get; set; } = "Cán bộ";

    public int DepartmentId { get; set; }

    public string Department { get; set; } = "";

    public bool IsActive { get; set; } = true;

    // Cán bộ có giấy phép lái xe và được đơn vị cho phép tự lái xe công vụ hay không
    public bool CanSelfDrive { get; set; } = false;

    public string Phone { get; set; } = "";

    public string Notes { get; set; } = "";
}
