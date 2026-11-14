namespace MyWebApp.Models.Enums;

/// <summary>
/// Trạng thái vòng đời chuyến công tác (Trip) theo đặc tả:
/// WaitingDeparture/Dispatched -> HandingOver -> InUse/InTransit -> Returned -> Completed
/// </summary>
public enum TripStatus
{
    WaitingDeparture,   // Chờ xuất phát (Đã điều phối)
    Dispatched,         // Đã điều phối
    HandingOver,        // Đang bàn giao xe
    InTransit,          // Đang di chuyển (tương thích code cũ)
    InUse,              // Đang sử dụng phương tiện
    NeedSupport,        // Cần hỗ trợ / gặp sự cố
    Overdue,            // Quá hạn
    Returned,           // Đã hoàn trả xe (Chờ quyết toán)
    Completed,          // Đã hoàn tất chuyến đi
    Cancelled           // Đã hủy lệnh điều động
}
