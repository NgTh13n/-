namespace MyWebApp.Models;

/// <summary>
/// Lịch sử chuyển tiếp trạng thái (Audit trail trong phiên chạy)
/// Theo dõi các bước: Nháp -> Chờ duyệt -> Đã duyệt -> Đã điều phối -> Bàn giao -> Hoàn trả...
/// </summary>
public class StatusHistory
{
    public int Id { get; set; }

    // Loại thực thể: "VehicleRequest", "Trip", "Vehicle", "MaintenanceRecord"
    public string EntityType { get; set; } = "";

    // Khóa Id của thực thể
    public int EntityId { get; set; }

    // Trạng thái ban đầu
    public string FromStatus { get; set; } = "";

    // Trạng thái mới
    public string ToStatus { get; set; } = "";

    // Thời điểm ghi nhận thay đổi
    public DateTime ChangedAt { get; set; } = DateTime.Now;

    // Cán bộ thực hiện thay đổi
    public int? ChangedByOfficerId { get; set; }

    public string ChangedByFullName { get; set; } = "";

    // Ý kiến / Ghi chú / Lý do (ví dụ lý do từ chối, lý do yêu cầu bổ sung)
    public string Comment { get; set; } = "";
}
