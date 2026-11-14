namespace MyWebApp.Models.Enums;

/// <summary>
/// Trạng thái yêu cầu sử dụng phương tiện CAND theo đặc tả Mục 4.1:
/// Draft -> PendingApproval -> (NeedAdjustment) -> Approved -> Dispatched -> InUse -> Completed
/// </summary>
public enum BookingStatus
{
    Draft,              // Bản nháp
    PendingApproval,    // Chờ phê duyệt
    NeedAdjustment,     // Yêu cầu bổ sung
    Rejected,           // Bị từ chối
    Approved,           // Đã duyệt (chờ điều phối)
    PendingDispatch,    // Chờ điều động (tương thích ngược với Approved)
    Dispatched,         // Đã điều phối (đã gán xe và người lái)
    HandingOver,        // Đang bàn giao xe
    InUse,              // Đang sử dụng phương tiện
    InProgress,         // Đang thực hiện (tương thích ngược với InUse)
    PendingSettlement,  // Chờ quyết toán
    Overdue,            // Quá hạn trả xe
    Completed,          // Đã hoàn thành (đã hoàn trả và quyết toán)
    Cancelled           // Đã hủy
}
