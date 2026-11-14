namespace MyWebApp.Models.Enums;

/// <summary>
/// Trạng thái hồ sơ bảo dưỡng / sửa chữa phương tiện
/// </summary>
public enum MaintenanceStatus
{
    Planned,    // Lên kế hoạch / Chờ đưa vào xưởng
    InProgress, // Đang bảo dưỡng / sửa chữa
    Completed,  // Đã hoàn thành, nghiệm thu đưa xe về Sẵn sàng
    Cancelled   // Đã hủy
}
