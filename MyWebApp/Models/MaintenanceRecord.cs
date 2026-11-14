using System.ComponentModel.DataAnnotations;
using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

/// <summary>
/// Hồ sơ bảo dưỡng, sửa chữa hoặc kiểm định kỹ thuật phương tiện
/// </summary>
public class MaintenanceRecord
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phương tiện.")]
    public int VehicleId { get; set; }

    public MaintenanceType MaintenanceType { get; set; } = MaintenanceType.Periodic;

    [Required(ErrorMessage = "Vui lòng nhập nội dung bảo dưỡng / sửa chữa.")]
    [StringLength(500, ErrorMessage = "Nội dung tối đa 500 ký tự.")]
    public string Description { get; set; } = "";

    // Ngày dự kiến hoặc ngày bắt đầu bảo dưỡng
    public DateTime PlannedAt { get; set; } = DateTime.Today;

    // Ngày hoàn thành bảo dưỡng và nghiệm thu đưa xe về Sẵn sàng
    public DateTime? CompletedAt { get; set; }

    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Planned;

    // Số km của xe tại thời điểm bảo dưỡng
    public double? OdometerKm { get; set; }

    // Đơn vị / Xưởng thực hiện bảo dưỡng (gara mô phỏng)
    public string GarageName { get; set; } = "Xưởng bảo dưỡng kỹ thuật CAND";

    // Chi phí dự kiến / thực tế (VNĐ)
    public decimal? Cost { get; set; }

    public string Notes { get; set; } = "";
}
