namespace MyWebApp.Models.Enums;

/// <summary>
/// Alias enum RequestStatus tương đương BookingStatus theo đặc tả SRS v1.0
/// </summary>
public enum RequestStatus
{
    Draft = BookingStatus.Draft,
    PendingApproval = BookingStatus.PendingApproval,
    NeedAdjustment = BookingStatus.NeedAdjustment,
    Rejected = BookingStatus.Rejected,
    Approved = BookingStatus.Approved,
    PendingDispatch = BookingStatus.PendingDispatch,
    Dispatched = BookingStatus.Dispatched,
    HandingOver = BookingStatus.HandingOver,
    InUse = BookingStatus.InUse,
    InProgress = BookingStatus.InProgress,
    PendingSettlement = BookingStatus.PendingSettlement,
    Overdue = BookingStatus.Overdue,
    Completed = BookingStatus.Completed,
    Cancelled = BookingStatus.Cancelled
}
