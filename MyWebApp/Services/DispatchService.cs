using MyWebApp.Data;
using MyWebApp.Helpers;
using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Services;

public class DispatchService
{
    private readonly MockDataService duLieu;
    private readonly AuthService authService;

    public DispatchService(MockDataService mockDataService, AuthService authService)
    {
        duLieu = mockDataService;
        this.authService = authService;
    }

    // Lấy danh sách các yêu cầu đang chờ điều động (đã được lãnh đạo duyệt)
    public List<Booking> GetPendingDispatchBookings()
    {
        List<Booking> ketQua = new List<Booking>();

        foreach (Booking b in duLieu.Bookings)
        {
            if (b.Status == BookingStatus.PendingDispatch || b.Status == BookingStatus.Approved)
            {
                ketQua.Add(SaoChepBooking(b));
            }
        }

        return ketQua;
    }

    // Lấy danh sách tất cả các lệnh điều động / chuyến công tác
    public List<Trip> GetAllDispatchOrders()
    {
        List<Trip> ketQua = new List<Trip>();

        foreach (Trip t in duLieu.Trips)
        {
            ketQua.Add(SaoChepTrip(t));
        }

        return ketQua;
    }

    // Lấy danh sách xe phù hợp với yêu cầu (đúng chủng loại, đủ chỗ ngồi, không bảo dưỡng)
    public List<Vehicle> GetEligibleVehicles(Booking don)
    {
        List<Vehicle> ketQua = new List<Vehicle>();

        foreach (Vehicle xe in duLieu.Vehicles)
        {
            // Bỏ qua xe bảo dưỡng hoặc ngừng hoạt động
            if (xe.Status == VehicleStatus.DangBaoDuong || xe.Status == VehicleStatus.NgungHoatDong)
            {
                continue;
            }

            // Kiểm tra loại phương tiện
            string tenLoaiXe = EnumHelper.TenVehicleType(xe.VehicleType);
            if (!string.Equals(tenLoaiXe, don.RequestedVehicleType, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Kiểm tra số chỗ ngồi
            if (xe.Seats < don.PassengerCount)
            {
                continue;
            }

            ketQua.Add(xe);
        }

        return ketQua;
    }

    // Lấy danh sách lái xe khả dụng (đang hoạt động, không nghỉ phép)
    public List<Driver> GetEligibleDrivers()
    {
        List<Driver> ketQua = new List<Driver>();

        foreach (Driver d in duLieu.Drivers)
        {
            if (d.Status == DriverStatus.Active)
            {
                ketQua.Add(d);
            }
        }

        return ketQua;
    }

    /// <summary>
    /// THUẬT TOÁN KIỂM TRA TRÙNG LỊCH PHƯƠNG TIỆN
    /// Hai khoảng thời gian [tuNgay, denNgay] và [don.DepartureTime, don.ReturnTime] trùng nhau khi:
    /// tuNgay < don.ReturnTime && don.DepartureTime < denNgay
    /// </summary>
    public bool KiemTraTrungLichXe(int vehicleId, DateTime tuNgay, DateTime denNgay, int? boQuaTripId, out string thongBaoLoi)
    {
        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (xe == null)
        {
            thongBaoLoi = "Không tìm thấy thông tin phương tiện.";
            return false;
        }

        if (xe.Status == VehicleStatus.DangBaoDuong)
        {
            thongBaoLoi = $"Phương tiện {xe.LicensePlate} đang bảo dưỡng, không thể điều động.";
            return false;
        }

        if (xe.Status == VehicleStatus.NgungHoatDong)
        {
            thongBaoLoi = $"Phương tiện {xe.LicensePlate} đã ngừng hoạt động.";
            return false;
        }

        foreach (Trip chuyen in duLieu.Trips)
        {
            if (boQuaTripId.HasValue && chuyen.Id == boQuaTripId.Value)
            {
                continue;
            }

            if (chuyen.VehicleId != vehicleId)
            {
                continue;
            }

            // Chuyến đã hoàn thành và quyết toán thì không tính trùng
            if (chuyen.IsSettled && chuyen.Status == TripStatus.Returned)
            {
                continue;
            }

            Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
            if (don == null || don.Status == BookingStatus.Cancelled || don.Status == BookingStatus.Rejected)
            {
                continue;
            }

            // Kiểm tra giao thoa khoảng thời gian
            if (tuNgay < don.ReturnTime && don.DepartureTime < denNgay)
            {
                thongBaoLoi = $"Phương tiện {xe.LicensePlate} ({xe.Brand} {xe.Model}) bị trùng lịch với chuyến công tác {don.BookingCode} (Từ {don.DepartureTime:dd/MM/yyyy HH:mm} đến {don.ReturnTime:dd/MM/yyyy HH:mm}).";
                return false;
            }
        }

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// THUẬT TOÁN KIỂM TRA TRÙNG LỊCH LÁI XE
    /// </summary>
    public bool KiemTraTrungLichLaiXe(int driverId, DateTime tuNgay, DateTime denNgay, int? boQuaTripId, out string thongBaoLoi)
    {
        Driver? laiXe = duLieu.Drivers.FirstOrDefault(d => d.Id == driverId);
        if (laiXe == null)
        {
            thongBaoLoi = "Không tìm thấy thông tin lái xe.";
            return false;
        }

        if (laiXe.Status == DriverStatus.OnLeave)
        {
            thongBaoLoi = $"Lái xe {laiXe.FullName} đang trong thời gian nghỉ phép.";
            return false;
        }

        if (laiXe.Status == DriverStatus.Inactive)
        {
            thongBaoLoi = $"Lái xe {laiXe.FullName} hiện không còn làm việc.";
            return false;
        }

        foreach (Trip chuyen in duLieu.Trips)
        {
            if (boQuaTripId.HasValue && chuyen.Id == boQuaTripId.Value)
            {
                continue;
            }

            if (chuyen.DriverId != driverId)
            {
                continue;
            }

            if (chuyen.IsSettled && chuyen.Status == TripStatus.Returned)
            {
                continue;
            }

            Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
            if (don == null || don.Status == BookingStatus.Cancelled || don.Status == BookingStatus.Rejected)
            {
                continue;
            }

            if (tuNgay < don.ReturnTime && don.DepartureTime < denNgay)
            {
                thongBaoLoi = $"Lái xe {laiXe.FullName} bị trùng lịch với chuyến công tác {don.BookingCode} (Từ {don.DepartureTime:dd/MM/yyyy HH:mm} đến {don.ReturnTime:dd/MM/yyyy HH:mm}).";
                return false;
            }
        }

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// LẬP LỆNH ĐIỀU ĐỘNG PHƯƠNG TIỆN
    /// Áp dụng đầy đủ 6 quy tắc nghiệp vụ điều động (Quy tắc 5, 6, 7, 8, 9, 10)
    /// </summary>
    public bool CreateDispatchOrder(int bookingId, int vehicleId, int driverId, out string thongBaoLoi)
    {
        if (!authService.CanDispatch)
        {
            thongBaoLoi = "Bạn không có quyền thực hiện điều động phương tiện.";
            return false;
        }

        User? nguoiLapLenh = authService.CurrentUser;
        if (nguoiLapLenh == null)
        {
            thongBaoLoi = "Vui lòng đăng nhập.";
            return false;
        }

        // 1. Kiểm tra yêu cầu (Quy tắc 5: Chỉ yêu cầu đã được duyệt mới được điều động)
        Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == bookingId);
        if (don == null)
        {
            thongBaoLoi = "Không tìm thấy yêu cầu đăng ký.";
            return false;
        }

        if (don.Status != BookingStatus.PendingDispatch && don.Status != BookingStatus.Approved)
        {
            thongBaoLoi = "Chỉ có thể điều động cho yêu cầu đã được lãnh đạo phê duyệt.";
            return false;
        }

        // 2. Quy tắc 10: Phải có cả phương tiện và lái xe
        if (vehicleId <= 0 || driverId <= 0)
        {
            thongBaoLoi = "Bắt buộc phải chọn cả phương tiện và lái xe để lập lệnh điều động.";
            return false;
        }

        // 3. Kiểm tra phương tiện (Quy tắc 6: Đúng loại xe; Quy tắc 7: Đủ số chỗ)
        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (xe == null)
        {
            thongBaoLoi = "Không tìm thấy phương tiện đã chọn.";
            return false;
        }

        string tenLoaiXe = EnumHelper.TenVehicleType(xe.VehicleType);
        if (!string.Equals(tenLoaiXe, don.RequestedVehicleType, StringComparison.OrdinalIgnoreCase))
        {
            thongBaoLoi = $"Phương tiện được chọn ({xe.LicensePlate} - {tenLoaiXe}) không đúng với loại xe cán bộ yêu cầu ({don.RequestedVehicleType}).";
            return false;
        }

        if (xe.Seats < don.PassengerCount)
        {
            thongBaoLoi = $"Số chỗ ngồi của xe {xe.LicensePlate} ({xe.Seats} chỗ) không đủ cho {don.PassengerCount} người tham gia.";
            return false;
        }

        // 4. Quy tắc 8: Kiểm tra trùng lịch phương tiện
        if (!KiemTraTrungLichXe(vehicleId, don.DepartureTime, don.ReturnTime, null, out string loiXe))
        {
            thongBaoLoi = loiXe;
            return false;
        }

        // 5. Quy tắc 9: Kiểm tra trùng lịch lái xe
        if (!KiemTraTrungLichLaiXe(driverId, don.DepartureTime, don.ReturnTime, null, out string loiLaiXe))
        {
            thongBaoLoi = loiLaiXe;
            return false;
        }

        Driver? laiXe = duLieu.Drivers.FirstOrDefault(d => d.Id == driverId);

        // 6. Cấp mã chuyến đi và mã lệnh điều động
        int idMoi = 1;
        foreach (Trip t in duLieu.Trips)
        {
            if (t.Id >= idMoi)
            {
                idMoi = t.Id + 1;
            }
        }

        string maLenh = $"LĐĐ-{DateTime.Today.Year}-{idMoi:D3}";

        Trip chuyenMoi = new Trip
        {
            Id = idMoi,
            DispatchCode = maLenh,
            BookingId = don.Id,
            VehicleId = xe.Id,
            DriverId = driverId,
            DispatcherId = nguoiLapLenh.Id,
            DispatchedAt = DateTime.Now,
            Status = TripStatus.WaitingDeparture,
            StartKm = xe.CurrentKm,
            EndKm = xe.CurrentKm,
            FuelUsed = 0,
            IsSettled = false
        };

        // Cập nhật trạng thái Booking và Phương tiện
        don.Status = BookingStatus.Dispatched;
        xe.Status = VehicleStatus.DangDuocDieuDong;

        duLieu.Trips.Add(chuyenMoi);

        // Ghi nhật ký hoạt động
        GhiNhatKy(nguoiLapLenh.Id, "Lập lệnh điều động",
            $"Cán bộ {nguoiLapLenh.FullName} đã lập lệnh {maLenh} phân công xe {xe.LicensePlate} và lái xe {laiXe?.FullName} cho yêu cầu {don.BookingCode}.");

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// HỦY LỆNH ĐIỀU ĐỘNG (Khi xe chưa xuất phát)
    /// </summary>
    public bool CancelDispatchOrder(int tripId, out string thongBaoLoi)
    {
        if (!authService.CanDispatch)
        {
            thongBaoLoi = "Bạn không có quyền hủy lệnh điều động.";
            return false;
        }

        Trip? chuyen = duLieu.Trips.FirstOrDefault(t => t.Id == tripId);
        if (chuyen == null)
        {
            thongBaoLoi = "Không tìm thấy lệnh điều động cần hủy.";
            return false;
        }

        if (chuyen.Status != TripStatus.WaitingDeparture)
        {
            thongBaoLoi = "Chuyến đi đã xuất phát hoặc đã hoàn thành, không thể hủy lệnh điều động.";
            return false;
        }

        Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
        if (don != null)
        {
            // Trả lại trạng thái Chờ điều động cho đơn
            don.Status = BookingStatus.PendingDispatch;
        }

        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == chuyen.VehicleId);
        if (xe != null)
        {
            xe.Status = VehicleStatus.SanSang;
        }

        duLieu.Trips.Remove(chuyen);

        User? user = authService.CurrentUser;
        if (user != null)
        {
            GhiNhatKy(user.Id, "Hủy lệnh điều động",
                $"Đã hủy lệnh điều động {chuyen.DispatchCode} cho yêu cầu {don?.BookingCode}. Đưa yêu cầu về trạng thái chờ điều động.");
        }

        thongBaoLoi = "";
        return true;
    }

    // Các hàm tra cứu thông tin hiển thị
    public Booking? GetBooking(int bookingId)
    {
        Booking? b = duLieu.Bookings.FirstOrDefault(x => x.Id == bookingId);
        return b != null ? SaoChepBooking(b) : null;
    }

    public Vehicle? GetVehicle(int vehicleId)
    {
        return duLieu.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
    }

    public Driver? GetDriver(int driverId)
    {
        return duLieu.Drivers.FirstOrDefault(d => d.Id == driverId);
    }

    public User? GetUser(int userId)
    {
        return duLieu.Users.FirstOrDefault(u => u.Id == userId);
    }

    private void GhiNhatKy(int userId, string action, string description)
    {
        int idMoi = 1;
        foreach (ActivityLog log in duLieu.ActivityLogs)
        {
            if (log.Id >= idMoi)
            {
                idMoi = log.Id + 1;
            }
        }

        duLieu.ActivityLogs.Add(new ActivityLog
        {
            Id = idMoi,
            UserId = userId,
            Action = action,
            Description = description,
            CreatedAt = DateTime.Now
        });
    }

    private Booking SaoChepBooking(Booking b)
    {
        return new Booking
        {
            Id = b.Id,
            BookingCode = b.BookingCode,
            RequesterId = b.RequesterId,
            DepartmentId = b.DepartmentId,
            WorkType = b.WorkType,
            Purpose = b.Purpose,
            Departure = b.Departure,
            Destination = b.Destination,
            DepartureTime = b.DepartureTime,
            ReturnTime = b.ReturnTime,
            PassengerCount = b.PassengerCount,
            RequestedVehicleType = b.RequestedVehicleType,
            Status = b.Status,
            RejectionReason = b.RejectionReason
        };
    }

    private Trip SaoChepTrip(Trip t)
    {
        return new Trip
        {
            Id = t.Id,
            DispatchCode = t.DispatchCode,
            BookingId = t.BookingId,
            VehicleId = t.VehicleId,
            DriverId = t.DriverId,
            DispatcherId = t.DispatcherId,
            DispatchedAt = t.DispatchedAt,
            Status = t.Status,
            ActualDeparture = t.ActualDeparture,
            ActualReturn = t.ActualReturn,
            StartKm = t.StartKm,
            EndKm = t.EndKm,
            FuelUsed = t.FuelUsed,
            IsSettled = t.IsSettled
        };
    }
}
