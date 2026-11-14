namespace MyWebApp.Models.Enums;

/// <summary>
/// Loại hình bảo dưỡng kỹ thuật phương tiện
/// </summary>
public enum MaintenanceType
{
    Periodic,   // Bảo dưỡng định kỳ theo số km / thời gian
    Repair,     // Sửa chữa đột xuất / khắc phục sự cố
    Inspection  // Đăng kiểm / kiểm định kỹ thuật an toàn
}
