namespace MyWebApp.Models.Enums;

/// <summary>
/// Trạng thái khai thác vận hành phương tiện:
/// - Ready: Sẵn sàng điều động
/// - Maintenance: Đang bảo dưỡng / sửa chữa (không được điều phối)
/// - Inactive: Ngừng hoạt động / Chờ thanh lý (không được điều phối)
/// </summary>
public enum VehicleOperationalStatus
{
    Ready,          // Sẵn sàng
    Maintenance,    // Đang bảo dưỡng
    Inactive        // Ngừng hoạt động
}
