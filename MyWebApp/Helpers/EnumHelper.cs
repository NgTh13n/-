using MyWebApp.Models.Enums;

namespace MyWebApp.Helpers;

// Helper tập trung: chuyển Enum sang tên tiếng Việt hiển thị trên giao diện.
// Thêm enum mới thì thêm hàm mới vào đây.
public static class EnumHelper
{
    public static string TenVehicleStatus(VehicleStatus status)
    {
        if (status == VehicleStatus.SanSang)
        {
            return "Sẵn sàng";
        }

        if (status == VehicleStatus.DangDuocDieuDong)
        {
            return "Đang được điều động";
        }

        if (status == VehicleStatus.DangBaoDuong)
        {
            return "Đang bảo dưỡng";
        }

        if (status == VehicleStatus.NgungHoatDong)
        {
            return "Ngừng hoạt động";
        }

        return "";
    }

    public static string TenVehicleType(VehicleType type)
    {
        if (type == VehicleType.OTo)
        {
            return "Ô tô";
        }

        if (type == VehicleType.XeMay)
        {
            return "Xe máy";
        }

        if (type == VehicleType.XeTai)
        {
            return "Xe tải";
        }

        if (type == VehicleType.XeChuyenDung)
        {
            return "Xe chuyên dụng";
        }

        return "";
    }

    public static string TenDriverStatus(DriverStatus status)
    {
        if (status == DriverStatus.Active)
        {
            return "Đang làm việc";
        }

        if (status == DriverStatus.OnLeave)
        {
            return "Đang nghỉ phép";
        }

        if (status == DriverStatus.Inactive)
        {
            return "Không còn làm việc";
        }

        return "";
    }

    public static string TenBookingStatus(BookingStatus status)
    {
        if (status == BookingStatus.Draft)
        {
            return "Bản nháp";
        }

        if (status == BookingStatus.PendingApproval)
        {
            return "Chờ phê duyệt";
        }

        if (status == BookingStatus.Rejected)
        {
            return "Bị từ chối";
        }

        if (status == BookingStatus.Approved)
        {
            return "Đã duyệt";
        }

        if (status == BookingStatus.PendingDispatch)
        {
            return "Chờ điều động";
        }

        if (status == BookingStatus.Dispatched)
        {
            return "Đã điều động";
        }

        if (status == BookingStatus.InProgress)
        {
            return "Đang thực hiện";
        }

        if (status == BookingStatus.PendingSettlement)
        {
            return "Chờ quyết toán";
        }

        if (status == BookingStatus.Completed)
        {
            return "Hoàn thành";
        }

        if (status == BookingStatus.Cancelled)
        {
            return "Đã hủy";
        }

        return "";
    }

    public static string TenTripStatus(TripStatus status)
    {
        if (status == TripStatus.WaitingDeparture)
        {
            return "Chờ xuất phát";
        }

        if (status == TripStatus.InTransit)
        {
            return "Đang di chuyển";
        }

        if (status == TripStatus.NeedSupport)
        {
            return "Cần hỗ trợ";
        }

        if (status == TripStatus.Returned)
        {
            return "Đã về";
        }

        return "";
    }

    public static string TenUserRole(UserRole role)
    {
        if (role == UserRole.Officer)
        {
            return "Cán bộ / Chiến sĩ";
        }

        if (role == UserRole.Leader)
        {
            return "Lãnh đạo";
        }

        if (role == UserRole.FleetManager)
        {
            return "Quản lý đội xe";
        }

        if (role == UserRole.Administrator)
        {
            return "Quản trị viên";
        }

        return "";
    }
}
