using MyWebApp.Models.Enums;

namespace MyWebApp.Helpers;

// Helper tập trung: chuyển Enum sang tên tiếng Việt hiển thị trên giao diện CAND.
public static class EnumHelper
{
    public static string TenVehicleStatus(VehicleStatus status)
    {
        return status switch
        {
            VehicleStatus.SanSang => "Sẵn sàng",
            VehicleStatus.DangDuocDieuDong => "Đang được điều động",
            VehicleStatus.DangBaoDuong => "Đang bảo dưỡng",
            VehicleStatus.NgungHoatDong => "Ngừng hoạt động",
            _ => "Không xác định"
        };
    }

    public static string TenVehicleOperationalStatus(VehicleOperationalStatus status)
    {
        return status switch
        {
            VehicleOperationalStatus.Ready => "Sẵn sàng",
            VehicleOperationalStatus.Maintenance => "Đang bảo dưỡng",
            VehicleOperationalStatus.Inactive => "Ngừng hoạt động",
            VehicleOperationalStatus.InUse => "Đang được điều động",
            _ => "Không xác định"
        };
    }

    public static string TenVehicleType(VehicleType type)
    {
        return type switch
        {
            VehicleType.OTo => "Ô tô",
            VehicleType.XeMay => "Xe máy",
            VehicleType.XeTai => "Xe tải",
            VehicleType.XeChuyenDung => "Xe chuyên dụng",
            _ => "Phương tiện khác"
        };
    }

    public static string TenDriverStatus(DriverStatus status)
    {
        return status switch
        {
            DriverStatus.Active => "Đang làm việc",
            DriverStatus.OnLeave => "Đang nghỉ phép",
            DriverStatus.Inactive => "Không còn làm việc",
            _ => "Không xác định"
        };
    }

    public static string TenDriverMode(DriverMode mode)
    {
        return mode switch
        {
            DriverMode.AssignedDriver => "Lái xe đơn vị",
            DriverMode.SelfDrive => "Cán bộ tự lái",
            _ => "Lái xe đơn vị"
        };
    }

    public static string TenBookingStatus(BookingStatus status)
    {
        return status switch
        {
            BookingStatus.Draft => "Bản nháp",
            BookingStatus.PendingApproval => "Chờ phê duyệt",
            BookingStatus.NeedAdjustment => "Yêu cầu bổ sung",
            BookingStatus.Rejected => "Bị từ chối",
            BookingStatus.Approved => "Đã duyệt",
            BookingStatus.PendingDispatch => "Chờ điều động",
            BookingStatus.Dispatched => "Đã điều phối",
            BookingStatus.HandingOver => "Đang bàn giao",
            BookingStatus.InUse => "Đang sử dụng",
            BookingStatus.InProgress => "Đang thực hiện",
            BookingStatus.PendingSettlement => "Chờ quyết toán",
            BookingStatus.Overdue => "Quá hạn",
            BookingStatus.Completed => "Hoàn thành",
            BookingStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
    }

    public static string TenTripStatus(TripStatus status)
    {
        return status switch
        {
            TripStatus.WaitingDeparture => "Chờ xuất phát",
            TripStatus.Dispatched => "Đã điều phối",
            TripStatus.HandingOver => "Đang bàn giao",
            TripStatus.InTransit => "Đang di chuyển",
            TripStatus.InUse => "Đang sử dụng",
            TripStatus.NeedSupport => "Cần hỗ trợ",
            TripStatus.Overdue => "Quá hạn",
            TripStatus.Returned => "Đã hoàn trả",
            TripStatus.Completed => "Đã hoàn tất",
            TripStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
    }

    public static string TenMaintenanceStatus(MaintenanceStatus status)
    {
        return status switch
        {
            MaintenanceStatus.Planned => "Lên kế hoạch",
            MaintenanceStatus.InProgress => "Đang bảo dưỡng",
            MaintenanceStatus.Completed => "Đã hoàn tất",
            MaintenanceStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
    }

    public static string TenMaintenanceType(MaintenanceType type)
    {
        return type switch
        {
            MaintenanceType.Periodic => "Định kỳ",
            MaintenanceType.Repair => "Sửa chữa",
            MaintenanceType.Inspection => "Kiểm định / Đăng kiểm",
            _ => "Bảo dưỡng"
        };
    }

    public static string TenUserRole(UserRole role)
    {
        return role switch
        {
            UserRole.Officer => "Cán bộ / Chiến sĩ",
            UserRole.Leader => "Lãnh đạo đơn vị",
            UserRole.FleetManager => "Quản lý đội xe",
            UserRole.Administrator => "Quản trị viên",
            _ => "Người dùng"
        };
    }

    // =========================================================================
    // PONYTAIL EXTENSION METHODS: Gom toàn bộ CSS Badge & Tên tiếng Việt về 1 nơi
    // =========================================================================

    public static string BadgeClass(this BookingStatus status) => status switch
    {
        BookingStatus.Draft => "badge-status-neutral",
        BookingStatus.PendingApproval or BookingStatus.NeedAdjustment => "badge-status-warning",
        BookingStatus.Approved or BookingStatus.PendingDispatch or BookingStatus.Dispatched => "badge-status-info",
        BookingStatus.HandingOver or BookingStatus.InUse or BookingStatus.InProgress => "badge-status-info",
        BookingStatus.Completed => "badge-status-success",
        BookingStatus.Rejected or BookingStatus.Overdue or BookingStatus.Cancelled => "badge-status-danger",
        _ => "badge-status-neutral"
    };

    public static string BadgeClass(this TripStatus status) => status switch
    {
        TripStatus.WaitingDeparture or TripStatus.Dispatched => "badge-status-warning",
        TripStatus.HandingOver or TripStatus.InTransit or TripStatus.InUse => "badge-status-info",
        TripStatus.NeedSupport or TripStatus.Overdue or TripStatus.Cancelled => "badge-status-danger",
        TripStatus.Returned or TripStatus.Completed => "badge-status-success",
        _ => "badge-status-neutral"
    };

    public static string BadgeClass(this VehicleStatus status) => status switch
    {
        VehicleStatus.SanSang => "badge-status-success",
        VehicleStatus.DangDuocDieuDong => "badge-status-info",
        VehicleStatus.DangBaoDuong => "badge-status-warning",
        VehicleStatus.NgungHoatDong => "badge-status-danger",
        _ => "badge-status-neutral"
    };

    public static string BadgeClass(this MaintenanceStatus status) => status switch
    {
        MaintenanceStatus.Planned => "badge-status-warning",
        MaintenanceStatus.InProgress => "badge-status-info",
        MaintenanceStatus.Completed => "badge-status-success",
        MaintenanceStatus.Cancelled => "badge-status-danger",
        _ => "badge-status-neutral"
    };

    public static string Ten(this BookingStatus s) => TenBookingStatus(s);
    public static string Ten(this TripStatus s) => TenTripStatus(s);
    public static string Ten(this VehicleStatus s) => TenVehicleStatus(s);
    public static string Ten(this MaintenanceStatus s) => TenMaintenanceStatus(s);
    public static string Ten(this DriverMode m) => TenDriverMode(m);
    public static string Ten(this UserRole r) => TenUserRole(r);
}
