using MyWebApp.Data;
using MyWebApp.Helpers;
using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Services;

/// <summary>
/// Dịch vụ quản lý và tạo thông báo hệ thống theo vai trò người dùng trong Học viện CAND.
/// Tự động cập nhật theo thời gian thực từ dữ liệu In-Memory của MockDataService.
/// </summary>
public class AppNotificationService
{
    private readonly MockDataService duLieu;
    private readonly AuthService authService;
    private readonly HashSet<string> _readIds = new();

    public event Action? OnChanged;

    public AppNotificationService(MockDataService mockDataService, AuthService authService)
    {
        duLieu = mockDataService;
        this.authService = authService;
    }

    public List<AppNotification> GetNotifications()
    {
        var user = authService.CurrentUser;
        if (user == null) return new();

        var list = new List<AppNotification>();

        switch (user.Role)
        {
            case UserRole.Leader:
                // 1. Các đơn đăng ký xe đang chờ Lãnh đạo phê duyệt
                var pendingApprovals = duLieu.Bookings
                    .Where(b => b.Status == BookingStatus.PendingApproval)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToList();

                foreach (var b in pendingApprovals)
                {
                    list.Add(new AppNotification
                    {
                        Id = $"lead-appr-{b.Id}",
                        Title = $"Chờ duyệt: {b.BookingCode}",
                        Message = $"Đ/c {LayTenCanBo(b.RequesterId)} đề xuất xe đi {b.Destination} ({b.WorkType})",
                        Icon = "assignment_turned_in",
                        IconColor = "#f59e0b",
                        TargetUrl = "/approvals",
                        CreatedAt = b.CreatedAt
                    });
                }
                break;

            case UserRole.FleetManager:
                // 1. Các đơn đã duyệt chờ Đội xe điều động phương tiện & lái xe
                var pendingDispatches = duLieu.Bookings
                    .Where(b => b.Status == BookingStatus.PendingDispatch || b.Status == BookingStatus.Approved)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToList();

                foreach (var b in pendingDispatches)
                {
                    list.Add(new AppNotification
                    {
                        Id = $"fleet-disp-{b.Id}",
                        Title = $"Chờ bố trí xe: {b.BookingCode}",
                        Message = $"Lãnh đạo đã duyệt. Bố trí xe đi {b.Destination} cho đ/c {LayTenCanBo(b.RequesterId)}",
                        Icon = "directions_car",
                        IconColor = "#2563eb",
                        TargetUrl = "/dispatch",
                        CreatedAt = b.CreatedAt
                    });
                }

                // 2. Cảnh báo phương tiện đang bảo dưỡng kỹ thuật
                var maintenanceVehicles = duLieu.Vehicles
                    .Where(v => v.OperationalStatus == VehicleOperationalStatus.Maintenance)
                    .ToList();

                foreach (var v in maintenanceVehicles)
                {
                    list.Add(new AppNotification
                    {
                        Id = $"fleet-maint-{v.Id}",
                        Title = $"Bảo dưỡng: {v.LicensePlate}",
                        Message = $"{v.Brand} {v.Model} đang trong xưởng sửa chữa & kiểm tra kỹ thuật định kỳ",
                        Icon = "build",
                        IconColor = "#ef4444",
                        TargetUrl = "/vehicles",
                        CreatedAt = DateTime.Today.AddHours(7)
                    });
                }

                // 3. Lệnh điều động chuẩn bị xuất phát
                var waitingTrips = duLieu.Trips
                    .Where(t => t.Status == TripStatus.WaitingDeparture)
                    .ToList();

                foreach (var t in waitingTrips)
                {
                    list.Add(new AppNotification
                    {
                        Id = $"fleet-trip-{t.Id}",
                        Title = $"Chuẩn bị xuất phát: {t.DispatchCode}",
                        Message = $"Xe {t.VehiclePlateSnapshot} sẵn sàng bàn giao chìa khóa và sổ nhật trình",
                        Icon = "schedule",
                        IconColor = "#10b981",
                        TargetUrl = "/trips",
                        CreatedAt = t.ScheduledStartAt.AddMinutes(-30)
                    });
                }
                break;

            case UserRole.Officer:
                int officerId = user.OfficerId ?? user.Id;

                // 1. Chuyến công tác đang diễn ra của cán bộ
                var activeTrips = duLieu.Trips
                    .Where(t => t.Status == TripStatus.InUse &&
                               (t.SelfDrivingOfficerId == officerId || duLieu.Bookings.Any(b => b.Id == t.BookingId && b.RequesterId == user.Id)))
                    .ToList();

                foreach (var t in activeTrips)
                {
                    list.Add(new AppNotification
                    {
                        Id = $"off-trip-active-{t.Id}",
                        Title = $"Nhiệm vụ đang thực hiện: {t.DispatchCode}",
                        Message = $"Phương tiện {t.VehiclePlateSnapshot} đang hoạt động trên tuyến. Nhớ cập nhật km khi về",
                        Icon = "near_me",
                        IconColor = "#0284c7",
                        TargetUrl = "/trips",
                        CreatedAt = t.ScheduledStartAt
                    });
                }

                // 2. Danh sách các yêu cầu của cán bộ này
                var myBookings = duLieu.Bookings
                    .Where(b => b.RequesterId == user.Id)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(4)
                    .ToList();

                foreach (var b in myBookings)
                {
                    var (icon, color) = b.Status switch
                    {
                        BookingStatus.Approved or BookingStatus.Dispatched => ("check_circle", "#10b981"),
                        BookingStatus.InUse => ("directions_car", "#0284c7"),
                        BookingStatus.NeedAdjustment => ("info", "#f59e0b"),
                        BookingStatus.Rejected or BookingStatus.Cancelled => ("cancel", "#ef4444"),
                        BookingStatus.Completed => ("task_alt", "#64748b"),
                        _ => ("schedule", "#64748b")
                    };

                    list.Add(new AppNotification
                    {
                        Id = $"off-book-{b.Id}-{(int)b.Status}",
                        Title = $"{b.BookingCode}: {b.Status.Ten()}",
                        Message = $"Tuyến: {b.Destination} ({b.DepartureTime:dd/MM HH:mm})",
                        Icon = icon,
                        IconColor = color,
                        TargetUrl = "/bookings",
                        CreatedAt = b.CreatedAt
                    });
                }
                break;

            case UserRole.Administrator:
                // 1. Tình trạng sẵn sàng phương tiện
                int readyCount = duLieu.Vehicles.Count(v => v.OperationalStatus == VehicleOperationalStatus.Ready);
                list.Add(new AppNotification
                {
                    Id = "admin-readiness",
                    Title = "Trạng thái sẵn sàng chiến đấu",
                    Message = $"{readyCount}/8 phương tiện đạt chuẩn an toàn kỹ thuật, sẵn sàng điều động 24/7",
                    Icon = "shield",
                    IconColor = "#1b2e4b",
                    TargetUrl = "/vehicles",
                    CreatedAt = DateTime.Today.AddHours(7).AddMinutes(30)
                });

                // 2. Yêu cầu đang chờ duyệt toàn viện
                int pendingTotal = duLieu.Bookings.Count(b => b.Status == BookingStatus.PendingApproval);
                if (pendingTotal > 0)
                {
                    list.Add(new AppNotification
                    {
                        Id = "admin-pending-approvals",
                        Title = $"Đang chờ phê duyệt ({pendingTotal} yêu cầu)",
                        Message = "Toàn viện hiện có các yêu cầu công tác chờ Lãnh đạo thẩm định",
                        Icon = "pending_actions",
                        IconColor = "#f59e0b",
                        TargetUrl = "/approvals",
                        CreatedAt = DateTime.Now.AddHours(-1)
                    });
                }

                // 3. Nhật ký an ninh gần đây
                list.Add(new AppNotification
                {
                    Id = "admin-audit-log",
                    Title = "Nhật ký hệ thống",
                    Message = $"Ghi nhận {duLieu.ActivityLogs.Count} lượt tương tác và chuyển tiếp trạng thái",
                    Icon = "history",
                    IconColor = "#475569",
                    TargetUrl = "/audit-log",
                    CreatedAt = DateTime.Now.AddMinutes(-45)
                });
                break;
        }

        // Cập nhật trạng thái đã đọc từ session
        foreach (var item in list)
        {
            item.IsRead = _readIds.Contains(item.Id);
        }

        // Ưu tiên chưa đọc lên trên, sau đó xếp theo thời gian mới nhất
        return list
            .OrderBy(x => x.IsRead)
            .ThenByDescending(x => x.CreatedAt)
            .ToList();
    }

    public int GetUnreadCount() => GetNotifications().Count(x => !x.IsRead);

    public void MarkAsRead(string id)
    {
        if (_readIds.Add(id))
        {
            OnChanged?.Invoke();
        }
    }

    public void MarkAllAsRead()
    {
        var items = GetNotifications();
        bool changed = false;
        foreach (var item in items)
        {
            if (_readIds.Add(item.Id))
            {
                changed = true;
            }
        }
        if (changed)
        {
            OnChanged?.Invoke();
        }
    }

    private string LayTenCanBo(int requesterId)
    {
        // Ưu tiên tra cứu trực tiếp theo OfficerId
        if (duLieu.Officers.FirstOrDefault(o => o.Id == requesterId) is { } cb)
            return cb.FullName;

        // Fallback tra cứu nếu requesterId lưu theo UserId
        if (duLieu.Users.FirstOrDefault(x => x.Id == requesterId) is { } u)
        {
            int targetId = u.OfficerId ?? u.Id;
            return duLieu.Officers.FirstOrDefault(o => o.Id == targetId)?.FullName ?? u.FullName;
        }

        return "Cán bộ";
    }
}
