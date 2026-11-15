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
    public List<Booking> GetPendingDispatchBookings() =>
        duLieu.Bookings.Where(b => b.Status == BookingStatus.PendingDispatch || b.Status == BookingStatus.Approved)
                       .Select(SaoChepBooking).ToList();

    // Lấy danh sách tất cả các lệnh điều động / chuyến công tác
    public List<Trip> GetAllDispatchOrders() =>
        duLieu.Trips.Select(SaoChepTrip).ToList();

    // Lấy danh sách xe phù hợp với yêu cầu (đúng chủng loại, đủ chỗ ngồi, không bảo dưỡng / ngừng hoạt động)
    public List<Vehicle> GetEligibleVehicles(Booking don) =>
        duLieu.Vehicles.Where(xe => 
            xe.OperationalStatus != VehicleOperationalStatus.Maintenance &&
            xe.OperationalStatus != VehicleOperationalStatus.Inactive &&
            string.Equals(EnumHelper.TenVehicleType(xe.VehicleType), don.RequestedVehicleType, StringComparison.OrdinalIgnoreCase) &&
            xe.Seats >= don.PassengerCount
        ).ToList();

    // Lấy danh sách lái xe khả dụng
    public List<Driver> GetEligibleDrivers() =>
        duLieu.Drivers.Where(d => d.Status == DriverStatus.Active && d.IsActive).ToList();

    // Lấy hồ sơ cán bộ theo RequesterId
    public Officer? GetOfficer(int requesterId)
    {
        // Ưu tiên tra cứu trực tiếp theo OfficerId
        if (duLieu.Officers.FirstOrDefault(o => o.Id == requesterId) is { } cb)
            return cb;

        // Fallback tra cứu nếu requesterId lưu theo UserId
        var u = duLieu.Users.FirstOrDefault(x => x.Id == requesterId);
        int targetId = u?.OfficerId ?? requesterId;
        return duLieu.Officers.FirstOrDefault(o => o.Id == targetId);
    }

    /// <summary>
    /// THUẬT TOÁN KIỂM TRA TRÙNG LỊCH PHƯƠNG TIỆN (Quy ước nửa mở [Start, End))
    /// Xung đột khi: tuNgay < chuyen.ScheduledEndAt && chuyen.ScheduledStartAt < denNgay
    /// </summary>
    public bool KiemTraTrungLichXe(int vehicleId, DateTime tuNgay, DateTime denNgay, int? boQuaTripId, out string thongBaoLoi)
    {
        var xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (xe == null)
        {
            thongBaoLoi = "Không tìm thấy thông tin phương tiện.";
            return false;
        }

        if (xe.OperationalStatus == VehicleOperationalStatus.Maintenance)
        {
            thongBaoLoi = $"Phương tiện {xe.LicensePlate} đang bảo dưỡng tại xưởng, không thể điều động.";
            return false;
        }

        if (xe.OperationalStatus == VehicleOperationalStatus.Inactive)
        {
            thongBaoLoi = $"Phương tiện {xe.LicensePlate} đã ngừng hoạt động.";
            return false;
        }

        var trungChuyen = duLieu.Trips.FirstOrDefault(chuyen =>
            (!boQuaTripId.HasValue || chuyen.Id != boQuaTripId.Value) &&
            chuyen.VehicleId == vehicleId &&
            !(chuyen.IsSettled && chuyen.Status == TripStatus.Returned) &&
            chuyen.Status != TripStatus.Completed && chuyen.Status != TripStatus.Cancelled &&
            tuNgay < chuyen.ScheduledEndAt && chuyen.ScheduledStartAt < denNgay &&
            duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId) is { } don &&
            don.Status != BookingStatus.Cancelled && don.Status != BookingStatus.Rejected
        );

        if (trungChuyen != null)
        {
            thongBaoLoi = $"Phương tiện {xe.LicensePlate} ({xe.Brand} {xe.Model}) bị trùng lịch với chuyến {trungChuyen.DispatchCode} (Từ {trungChuyen.ScheduledStartAt:dd/MM/yyyy HH:mm} đến {trungChuyen.ScheduledEndAt:dd/MM/yyyy HH:mm}).";
            return false;
        }

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// THUẬT TOÁN KIỂM TRA TRÙNG LỊCH LÁI XE ĐƠN VỊ
    /// </summary>
    public bool KiemTraTrungLichLaiXe(int driverId, DateTime tuNgay, DateTime denNgay, int? boQuaTripId, out string thongBaoLoi)
    {
        var laiXe = duLieu.Drivers.FirstOrDefault(d => d.Id == driverId);
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

        if (laiXe.Status == DriverStatus.Inactive || !laiXe.IsActive)
        {
            thongBaoLoi = $"Lái xe {laiXe.FullName} hiện không còn làm việc.";
            return false;
        }

        // Kiểm tra hạn bằng lái tại thời điểm bắt đầu chuyến
        if (laiXe.LicenseExpiryDate < tuNgay)
        {
            thongBaoLoi = $"CẢNH BÁO AN TOÀN: Giấy phép lái xe của đồng chí {laiXe.FullName} đã hết hạn vào ngày {laiXe.LicenseExpiryDate:dd/MM/yyyy}. Không đủ điều kiện tham gia điều khiển phương tiện!";
            return false;
        }

        var trungChuyen = duLieu.Trips.FirstOrDefault(chuyen =>
            (!boQuaTripId.HasValue || chuyen.Id != boQuaTripId.Value) &&
            chuyen.DriverId == driverId &&
            !(chuyen.IsSettled && chuyen.Status == TripStatus.Returned) &&
            chuyen.Status != TripStatus.Completed && chuyen.Status != TripStatus.Cancelled &&
            tuNgay < chuyen.ScheduledEndAt && chuyen.ScheduledStartAt < denNgay &&
            duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId) is { } don &&
            don.Status != BookingStatus.Cancelled && don.Status != BookingStatus.Rejected
        );

        if (trungChuyen != null)
        {
            thongBaoLoi = $"Lái xe {laiXe.FullName} bị trùng lịch với chuyến công tác {trungChuyen.DispatchCode} (Từ {trungChuyen.ScheduledStartAt:dd/MM/yyyy HH:mm} đến {trungChuyen.ScheduledEndAt:dd/MM/yyyy HH:mm}).";
            return false;
        }

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// KIỂM TRA TRÙNG LỊCH CÁN BỘ TỰ LÁI
    /// </summary>
    public bool KiemTraTrungLichCanBoTuLai(int officerId, DateTime tuNgay, DateTime denNgay, int? boQuaTripId, out string thongBaoLoi)
    {
        var cb = duLieu.Officers.FirstOrDefault(o => o.Id == officerId);
        if (cb == null)
        {
            thongBaoLoi = "Không tìm thấy hồ sơ cán bộ.";
            return false;
        }

        if (!cb.CanSelfDrive)
        {
            thongBaoLoi = $"CẢNH BÁO AN TOÀN: Cán bộ {cb.FullName} chưa được cấp quyền hoặc chưa đủ điều kiện tự lái xe công vụ (CanSelfDrive = False). Vui lòng chuyển sang phương án Lái xe đơn vị!";
            return false;
        }

        var trungChuyen = duLieu.Trips.FirstOrDefault(chuyen =>
            (!boQuaTripId.HasValue || chuyen.Id != boQuaTripId.Value) &&
            chuyen.DriverMode == DriverMode.SelfDrive &&
            chuyen.SelfDrivingOfficerId == officerId &&
            !(chuyen.IsSettled && chuyen.Status == TripStatus.Returned) &&
            chuyen.Status != TripStatus.Completed && chuyen.Status != TripStatus.Cancelled &&
            tuNgay < chuyen.ScheduledEndAt && chuyen.ScheduledStartAt < denNgay
        );

        if (trungChuyen != null)
        {
            thongBaoLoi = $"Cán bộ {cb.FullName} đang tự lái ở chuyến công tác khác ({trungChuyen.DispatchCode}) trong khoảng thời gian này.";
            return false;
        }

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// LẬP LỆNH ĐIỀU ĐỘNG PHƯƠNG TIỆN (Hỗ trợ 2 chế độ lái)
    /// </summary>
    public bool CreateDispatchOrder(int bookingId, int vehicleId, DriverMode driverMode, int driverId, out string thongBaoLoi)
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

        // 1. Kiểm tra yêu cầu (Chỉ yêu cầu đã được duyệt mới được điều động)
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

        // 2. Kiểm tra phương tiện
        if (vehicleId <= 0)
        {
            thongBaoLoi = "Vui lòng chọn phương tiện để điều động.";
            return false;
        }

        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (xe == null)
        {
            thongBaoLoi = "Không tìm thấy phương tiện đã chọn.";
            return false;
        }

        if (xe.OperationalStatus == VehicleOperationalStatus.Maintenance)
        {
            thongBaoLoi = $"Phương tiện {xe.LicensePlate} đang bảo dưỡng tại xưởng, không thể điều động.";
            return false;
        }

        if (xe.OperationalStatus == VehicleOperationalStatus.Inactive)
        {
            thongBaoLoi = $"Phương tiện {xe.LicensePlate} đã ngừng hoạt động.";
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

        // Kiểm tra trùng lịch xe
        if (!KiemTraTrungLichXe(vehicleId, don.DepartureTime, don.ReturnTime, null, out string loiXe))
        {
            thongBaoLoi = loiXe;
            return false;
        }

        // 3. Kiểm tra người lái theo từng chế độ
        string driverSnapshot = "";
        int? selfOfficerId = null;
        int finalDriverId = 0;

        if (driverMode == DriverMode.SelfDrive)
        {
            Officer? cb = GetOfficer(don.RequesterId);
            if (cb == null)
            {
                thongBaoLoi = "Không tìm thấy hồ sơ cán bộ đề nghị tự lái.";
                return false;
            }

            if (!cb.CanSelfDrive)
            {
                thongBaoLoi = $"CẢNH BÁO AN TOÀN: Cán bộ {cb.FullName} chưa đủ điều kiện tự lái xe công vụ (CanSelfDrive = False). Vui lòng chuyển sang phương án Lái xe đơn vị!";
                return false;
            }

            if (!KiemTraTrungLichCanBoTuLai(cb.Id, don.DepartureTime, don.ReturnTime, null, out string loiCB))
            {
                thongBaoLoi = loiCB;
                return false;
            }

            selfOfficerId = cb.Id;
            finalDriverId = 0;
            driverSnapshot = $"{cb.FullName} (Cán bộ tự lái)";
        }
        else
        {
            if (driverId <= 0)
            {
                thongBaoLoi = "Vui lòng chọn lái xe đơn vị để phân công.";
                return false;
            }

            Driver? laiXe = duLieu.Drivers.FirstOrDefault(d => d.Id == driverId);
            if (laiXe == null)
            {
                thongBaoLoi = "Không tìm thấy thông tin lái xe.";
                return false;
            }

            // Kiểm tra hạn GPLX
            if (laiXe.LicenseExpiryDate < don.DepartureTime)
            {
                thongBaoLoi = $"CẢNH BÁO AN TOÀN: Giấy phép lái xe của đồng chí {laiXe.FullName} đã hết hạn vào ngày {laiXe.LicenseExpiryDate:dd/MM/yyyy}. Vì an toàn giao thông, không thể phân công lái xe này!";
                return false;
            }

            // Kiểm tra trùng lịch lái xe
            if (!KiemTraTrungLichLaiXe(driverId, don.DepartureTime, don.ReturnTime, null, out string loiLaiXe))
            {
                thongBaoLoi = loiLaiXe;
                return false;
            }

            finalDriverId = driverId;
            driverSnapshot = laiXe.FullName;
        }

        // 4. Lập lệnh điều động (Trip)
        int idMoi = duLieu.Trips.Count > 0 ? duLieu.Trips.Max(x => x.Id) + 1 : 1;
        string maLenh = $"LĐĐ-{DateTime.Today.Year}-{idMoi:D3}";
        string tenNguoiYeuCau = GetUser(don.RequesterId)?.FullName ?? "Cán bộ đề nghị";

        Trip chuyenMoi = new Trip
        {
            Id = idMoi,
            DispatchCode = maLenh,
            BookingId = don.Id,
            VehicleId = xe.Id,
            DriverMode = driverMode,
            DriverId = finalDriverId,
            SelfDrivingOfficerId = selfOfficerId,
            DriverNameSnapshot = driverSnapshot,
            RequesterNameSnapshot = tenNguoiYeuCau,
            VehiclePlateSnapshot = xe.LicensePlate,
            ScheduledStartAt = don.DepartureTime,
            ScheduledEndAt = don.ReturnTime,
            DispatcherId = nguoiLapLenh.Id,
            DispatchedAt = DateTime.Now,
            Status = TripStatus.WaitingDeparture,
            StartKm = xe.CurrentKm,
            EndKm = xe.CurrentKm,
            FuelUsed = 0,
            IsSettled = false,
            Note = $"Lệnh điều động phục vụ nhiệm vụ: {don.Purpose}."
        };

        // Cập nhật trạng thái Booking
        don.Status = BookingStatus.Dispatched;
        duLieu.Trips.Add(chuyenMoi);

        // Ghi Audit Trail
        duLieu.AddStatusHistory(
            "Trip",
            idMoi,
            "None",
            "WaitingDeparture",
            nguoiLapLenh.OfficerId ?? nguoiLapLenh.Id,
            nguoiLapLenh.FullName,
            $"Lập lệnh điều động {maLenh} phân công xe {xe.LicensePlate} cho {driverSnapshot}.");

        duLieu.AddStatusHistory(
            "VehicleRequest",
            don.Id,
            "Approved",
            "Dispatched",
            nguoiLapLenh.OfficerId ?? nguoiLapLenh.Id,
            nguoiLapLenh.FullName,
            $"Đã điều phối xe {xe.LicensePlate} ({driverSnapshot}) theo lệnh {maLenh}.");

        GhiNhatKy(nguoiLapLenh.Id, "Lập lệnh điều động",
            $"Cán bộ {nguoiLapLenh.FullName} đã lập lệnh {maLenh} phân công xe {xe.LicensePlate} và {driverSnapshot} cho yêu cầu {don.BookingCode}.");

        thongBaoLoi = "";
        return true;
    }

    // Overload cũ tương thích
    public bool CreateDispatchOrder(int bookingId, int vehicleId, int driverId, out string thongBaoLoi)
    {
        Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == bookingId);
        DriverMode mode = don?.PreferredDriverMode ?? DriverMode.AssignedDriver;
        return CreateDispatchOrder(bookingId, vehicleId, mode, driverId, out thongBaoLoi);
    }

    /// <summary>
    /// HỦY LỆNH ĐIỀU ĐỘNG (Khi xe chưa bàn giao / chưa xuất phát)
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

        if (chuyen.Status != TripStatus.WaitingDeparture && chuyen.Status != TripStatus.Dispatched)
        {
            thongBaoLoi = "Chuyến đi đã xuất phát hoặc đã hoàn thành, không thể hủy lệnh điều động.";
            return false;
        }

        Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
        if (don != null)
        {
            don.Status = BookingStatus.Approved;
        }

        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == chuyen.VehicleId);

        duLieu.Trips.Remove(chuyen);

        User? user = authService.CurrentUser;
        if (user != null)
        {
            duLieu.AddStatusHistory(
                "Trip",
                tripId,
                chuyen.Status.ToString(),
                "Cancelled",
                user.OfficerId ?? user.Id,
                user.FullName,
                $"Hủy lệnh điều động {chuyen.DispatchCode}. Đưa yêu cầu về trạng thái chờ điều động.");

            GhiNhatKy(user.Id, "Hủy lệnh điều động",
                $"Đã hủy lệnh điều động {chuyen.DispatchCode} cho yêu cầu {don?.BookingCode}. Đưa yêu cầu về trạng thái chờ điều động.");
        }

        thongBaoLoi = "";
        return true;
    }

    // Các hàm tra cứu thông tin hiển thị
    public Booking? GetBooking(int bookingId) =>
        duLieu.Bookings.FirstOrDefault(x => x.Id == bookingId) is { } b ? SaoChepBooking(b) : null;

    public Vehicle? GetVehicle(int vehicleId) => duLieu.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

    public Driver? GetDriver(int driverId) => duLieu.Drivers.FirstOrDefault(d => d.Id == driverId);

    public User? GetUser(int userId) => duLieu.Users.FirstOrDefault(u => u.Id == userId);

    private void GhiNhatKy(int userId, string action, string description)
    {
        int idMoi = duLieu.ActivityLogs.Count > 0 ? duLieu.ActivityLogs.Max(x => x.Id) + 1 : 1;

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
            DepartmentSnapshot = b.DepartmentSnapshot,
            WorkType = b.WorkType,
            Purpose = b.Purpose,
            TaskDescription = b.TaskDescription,
            Departure = b.Departure,
            Destination = b.Destination,
            DepartureTime = b.DepartureTime,
            ReturnTime = b.ReturnTime,
            PassengerCount = b.PassengerCount,
            RequestedVehicleType = b.RequestedVehicleType,
            PreferredDriverMode = b.PreferredDriverMode,
            Status = b.Status,
            RejectionReason = b.RejectionReason,
            NeedAdjustmentReason = b.NeedAdjustmentReason,
            ApprovalOfficerId = b.ApprovalOfficerId,
            ApprovalAt = b.ApprovalAt,
            ApprovalComment = b.ApprovalComment,
            CancellationReason = b.CancellationReason,
            CreatedAt = b.CreatedAt,
            SubmittedAt = b.SubmittedAt,
            Notes = b.Notes
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
            DriverMode = t.DriverMode,
            DriverId = t.DriverId,
            SelfDrivingOfficerId = t.SelfDrivingOfficerId,
            DriverNameSnapshot = t.DriverNameSnapshot,
            RequesterNameSnapshot = t.RequesterNameSnapshot,
            VehiclePlateSnapshot = t.VehiclePlateSnapshot,
            ScheduledStartAt = t.ScheduledStartAt,
            ScheduledEndAt = t.ScheduledEndAt,
            DispatcherId = t.DispatcherId,
            DispatchedAt = t.DispatchedAt,
            Status = t.Status,
            HandoverAt = t.HandoverAt,
            StartKm = t.StartKm,
            FuelStartLevel = t.FuelStartLevel,
            ConditionBefore = t.ConditionBefore,
            ReturnedAt = t.ReturnedAt,
            EndKm = t.EndKm,
            FuelEndLevel = t.FuelEndLevel,
            ConditionAfter = t.ConditionAfter,
            HasIncident = t.HasIncident,
            IncidentNote = t.IncidentNote,
            FuelUsed = t.FuelUsed,
            IsSettled = t.IsSettled,
            SettledAt = t.SettledAt,
            SettledBy = t.SettledBy,
            Note = t.Note
        };
    }
}
