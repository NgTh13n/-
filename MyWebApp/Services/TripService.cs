using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Services;

public class TripService
{
    private readonly MockDataService duLieu;
    private readonly AuthService authService;

    public TripService(MockDataService mockDataService, AuthService authService)
    {
        duLieu = mockDataService;
        this.authService = authService;
    }

    // Lấy toàn bộ danh sách chuyến công tác (phân quyền xem)
    public List<Trip> GetAllTrips()
    {
        var u = authService.CurrentUser;
        if (u == null) return new();
        var q = u.Role == UserRole.Officer
            ? duLieu.Trips.Where(t => duLieu.Bookings.FirstOrDefault(b => b.Id == t.BookingId)?.RequesterId == u.Id)
            : duLieu.Trips;
        return q.Select(SaoChepTrip).ToList();
    }

    // Lấy danh sách các chuyến đi cần theo dõi (chưa hoàn tất)
    public List<Trip> GetActiveTrips() =>
        duLieu.Trips.Where(t => !t.IsSettled && t.Status != TripStatus.Completed && t.Status != TripStatus.Cancelled)
                    .Select(SaoChepTrip).ToList();

    // Lấy danh sách chuyến đang chờ bàn giao xuất phát
    public List<Trip> GetPendingHandoverTrips() =>
        duLieu.Trips.Where(t => t.Status == TripStatus.WaitingDeparture || t.Status == TripStatus.Dispatched)
                    .Select(SaoChepTrip).ToList();

    // Lấy danh sách chuyến đang lưu thông trên đường / đang sử dụng
    public List<Trip> GetInUseTrips() =>
        duLieu.Trips.Where(t => t.Status == TripStatus.InUse || t.Status == TripStatus.InTransit || t.Status == TripStatus.NeedSupport || t.Status == TripStatus.Overdue)
                    .Select(SaoChepTrip).ToList();

    // Lấy danh sách các chuyến đã hoàn tất
    public List<Trip> GetCompletedTrips() =>
        duLieu.Trips.Where(t => t.IsSettled || t.Status == TripStatus.Completed)
                    .Select(SaoChepTrip).ToList();

    // Lấy danh sách các chuyến đã trở về và đang chờ quyết toán (tương thích cũ)
    public List<Trip> GetPendingSettlementTrips() =>
        duLieu.Trips.Where(t => t.Status == TripStatus.Returned && !t.IsSettled)
                    .Select(SaoChepTrip).ToList();

    public List<Trip> GetSettledTrips() => GetCompletedTrips();

    /// <summary>
    /// BÀN GIAO PHƯƠNG TIỆN (Handover):
    /// Ghi nhận giờ giao, số km đi, nhiên liệu ban đầu, tình trạng xe -> Chuyển sang Đang sử dụng (InUse)
    /// </summary>
    public bool HandoverVehicle(int tripId, double startKm, string fuelStartLevel, string conditionBefore, out string thongBaoLoi)
    {
        User? nguoiDung = authService.CurrentUser;
        if (nguoiDung == null)
        {
            thongBaoLoi = "Vui lòng đăng nhập.";
            return false;
        }

        Trip? chuyen = duLieu.Trips.FirstOrDefault(t => t.Id == tripId);
        if (chuyen == null)
        {
            thongBaoLoi = "Không tìm thấy chuyến công tác cần bàn giao.";
            return false;
        }

        if (chuyen.Status != TripStatus.WaitingDeparture && chuyen.Status != TripStatus.Dispatched)
        {
            thongBaoLoi = "Chuyến đi này không ở trạng thái chờ bàn giao xuất phát.";
            return false;
        }

        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == chuyen.VehicleId);
        if (xe == null)
        {
            thongBaoLoi = "Không tìm thấy thông tin phương tiện.";
            return false;
        }

        if (xe.OperationalStatus == VehicleOperationalStatus.Maintenance || xe.OperationalStatus == VehicleOperationalStatus.Inactive)
        {
            thongBaoLoi = $"Phương tiện {xe.LicensePlate} đang bị khóa bảo dưỡng hoặc ngừng hoạt động, không thể bàn giao!";
            return false;
        }

        if (startKm < 0)
        {
            thongBaoLoi = "Chỉ số Km ban đầu không được âm.";
            return false;
        }

        // Cập nhật thông tin bàn giao vào Trip
        chuyen.HandoverAt = DateTime.Now;
        chuyen.StartKm = startKm;
        chuyen.FuelStartLevel = string.IsNullOrWhiteSpace(fuelStartLevel) ? "100%" : fuelStartLevel;
        chuyen.ConditionBefore = string.IsNullOrWhiteSpace(conditionBefore) ? "Xe bình thường, đầy đủ trang bị" : conditionBefore;
        chuyen.Status = TripStatus.InUse;

        // Cập nhật trạng thái Booking
        Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
        if (don != null)
        {
            don.Status = BookingStatus.InUse;
        }

        xe.Status = VehicleStatus.DangDuocDieuDong;

        // Ghi Audit Trail
        duLieu.AddStatusHistory(
            "Trip",
            chuyen.Id,
            "WaitingDeparture",
            "InUse",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            $"Bàn giao xe {xe.LicensePlate} cho {chuyen.DriverNameSnapshot}. Km đi: {startKm:N0} km, Nhiên liệu: {chuyen.FuelStartLevel}. Tình trạng: {chuyen.ConditionBefore}");

        if (don != null)
        {
            duLieu.AddStatusHistory(
                "VehicleRequest",
                don.Id,
                "Dispatched",
                "InUse",
                nguoiDung.OfficerId ?? nguoiDung.Id,
                nguoiDung.FullName,
                $"Phương tiện đã bàn giao và bắt đầu thực hiện nhiệm vụ.");
        }

        GhiNhatKy(nguoiDung.Id, "Bàn giao phương tiện",
            $"Cán bộ {nguoiDung.FullName} đã xác nhận biên bản bàn giao xe {xe.LicensePlate} cho chuyến {chuyen.DispatchCode}. Km bắt đầu: {startKm:N0} km.");

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// HOÀN TRẢ PHƯƠNG TIỆN (Return):
    /// Ghi nhận giờ trả, số km về (Km về >= Km đi), nhiên liệu còn lại, nhiên liệu tiêu thụ, sự cố (nếu có)
    /// -> Giải phóng xe và hoàn tất hồ sơ chuyến đi
    /// </summary>
    public bool ReturnVehicle(
        int tripId, 
        double endKm, 
        string fuelEndLevel, 
        double fuelUsed, 
        string conditionAfter, 
        bool hasIncident, 
        string incidentNote, 
        string generalNote, 
        out string thongBaoLoi)
    {
        User? nguoiDung = authService.CurrentUser;
        if (nguoiDung == null)
        {
            thongBaoLoi = "Vui lòng đăng nhập.";
            return false;
        }

        Trip? chuyen = duLieu.Trips.FirstOrDefault(t => t.Id == tripId);
        if (chuyen == null)
        {
            thongBaoLoi = "Không tìm thấy thông tin chuyến công tác.";
            return false;
        }

        if (chuyen.Status == TripStatus.WaitingDeparture || chuyen.Status == TripStatus.Dispatched)
        {
            thongBaoLoi = "Chuyến đi chưa bàn giao xuất phát, không thể lập biên bản hoàn trả.";
            return false;
        }

        if (chuyen.IsSettled || chuyen.Status == TripStatus.Completed)
        {
            thongBaoLoi = "Chuyến đi này đã hoàn tất thủ tục hoàn trả trước đó.";
            return false;
        }

        // Ràng buộc an ninh toàn vẹn: Km về >= Km đi
        if (endKm < chuyen.StartKm)
        {
            thongBaoLoi = $"Số Km kết thúc ({endKm:N0} km) không được nhỏ hơn số Km bàn giao ban đầu ({chuyen.StartKm:N0} km).";
            return false;
        }

        if (fuelUsed < 0)
        {
            thongBaoLoi = "Nhiên liệu tiêu thụ không được âm.";
            return false;
        }

        double quangDuong = endKm - chuyen.StartKm;

        // Cập nhật thông tin hoàn trả vào Trip
        chuyen.ReturnedAt = DateTime.Now;
        chuyen.EndKm = endKm;
        chuyen.FuelEndLevel = string.IsNullOrWhiteSpace(fuelEndLevel) ? "80%" : fuelEndLevel;
        chuyen.FuelUsed = fuelUsed;
        chuyen.ConditionAfter = string.IsNullOrWhiteSpace(conditionAfter) ? "Bình thường, không trầy xước" : conditionAfter;
        chuyen.HasIncident = hasIncident;
        chuyen.IncidentNote = hasIncident ? incidentNote : "";
        chuyen.IsSettled = true;
        chuyen.Status = TripStatus.Completed;

        if (!string.IsNullOrWhiteSpace(generalNote))
        {
            chuyen.Note = string.IsNullOrWhiteSpace(chuyen.Note) ? generalNote : $"{chuyen.Note} | {generalNote}";
        }

        // Cập nhật Booking -> Hoàn thành
        Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
        if (don != null)
        {
            don.Status = BookingStatus.Completed;
        }

        // Cập nhật xe: lưu chỉ số Odometer mới và cập nhật trạng thái kỹ thuật
        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == chuyen.VehicleId);
        if (xe != null)
        {
            xe.CurrentKm = endKm;
            if (hasIncident)
            {
                xe.OperationalStatus = VehicleOperationalStatus.Maintenance;
                xe.Status = VehicleStatus.DangBaoDuong;
                xe.Note = string.IsNullOrWhiteSpace(xe.Note)
                    ? $"Cần bảo dưỡng sau sự cố: {incidentNote}"
                    : $"{xe.Note} | Cần bảo dưỡng: {incidentNote}";

                int nextMaintId = duLieu.MaintenanceRecords.Count > 0 ? duLieu.MaintenanceRecords.Max(m => m.Id) + 1 : 1;
                duLieu.MaintenanceRecords.Add(new MaintenanceRecord
                {
                    Id = nextMaintId,
                    VehicleId = xe.Id,
                    MaintenanceType = MaintenanceType.Repair,
                    Description = $"Sửa chữa, khắc phục sau sự cố chuyến đi {chuyen.DispatchCode}: {incidentNote}",
                    PlannedAt = DateTime.Now,
                    Status = MaintenanceStatus.InProgress,
                    OdometerKm = endKm,
                    GarageName = "Xưởng bảo dưỡng kỹ thuật CAND",
                    Notes = $"Phát sinh từ chuyến đi {chuyen.DispatchCode}"
                });
            }
            else
            {
                xe.OperationalStatus = VehicleOperationalStatus.Ready;
                xe.Status = VehicleStatus.SanSang;
            }
        }

        // Ghi Audit Trail
        string ghiChuAudit = $"Nghiệm thu hoàn trả xe {xe?.LicensePlate}. Km về: {endKm:N0} km (Quãng đường: {quangDuong:N0} km, Xăng: {fuelUsed:N1}L).";
        if (hasIncident)
        {
            ghiChuAudit += $" [SỰ CỐ PHÁT SINH]: {incidentNote}";
        }

        duLieu.AddStatusHistory(
            "Trip",
            chuyen.Id,
            "InUse",
            "Completed",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            ghiChuAudit);

        if (don != null)
        {
            duLieu.AddStatusHistory(
                "VehicleRequest",
                don.Id,
                "InUse",
                "Completed",
                nguoiDung.OfficerId ?? nguoiDung.Id,
                nguoiDung.FullName,
                $"Hoàn tất chuyến công tác và quyết toán phương tiện.");
        }

        GhiNhatKy(nguoiDung.Id, "Nghiệm thu hoàn trả xe",
            $"Cán bộ {nguoiDung.FullName} đã hoàn tất nghiệm thu hoàn trả xe {xe?.LicensePlate} cho chuyến {chuyen.DispatchCode}. Quãng đường: {quangDuong:N0} km.");

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// BÁO CÁO SỰ CỐ / YÊU CẦU HỖ TRỢ TRÊN ĐƯỜNG CÔNG TÁC
    /// </summary>
    public bool ReportIncident(int tripId, string noiDungSuCo, out string thongBaoLoi)
    {
        User? nguoiDung = authService.CurrentUser;
        if (nguoiDung == null)
        {
            thongBaoLoi = "Vui lòng đăng nhập.";
            return false;
        }

        Trip? chuyen = duLieu.Trips.FirstOrDefault(t => t.Id == tripId);
        if (chuyen == null)
        {
            thongBaoLoi = "Không tìm thấy chuyến công tác.";
            return false;
        }

        if (chuyen.IsSettled || chuyen.Status == TripStatus.Completed)
        {
            thongBaoLoi = "Chuyến đi đã hoàn tất, không thể báo sự cố.";
            return false;
        }

        chuyen.Status = TripStatus.NeedSupport;
        chuyen.HasIncident = true;
        chuyen.IncidentNote = noiDungSuCo;
        chuyen.Note = $"[Cần hỗ trợ {DateTime.Now:HH:mm dd/MM}]: {noiDungSuCo} | {chuyen.Note}";

        duLieu.AddStatusHistory(
            "Trip",
            tripId,
            "InUse",
            "NeedSupport",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            $"Phát tín hiệu cần hỗ trợ khẩn cấp: {noiDungSuCo}");

        GhiNhatKy(nguoiDung.Id, "Báo sự cố chuyến đi",
            $"Chuyến {chuyen.DispatchCode} phát tín hiệu cần hỗ trợ: {noiDungSuCo}");

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// KHẮC PHỤC SỰ CỐ, TIẾP TỤC HÀNH TRÌNH
    /// </summary>
    public bool ResumeTrip(int tripId, string ghiChu, out string thongBaoLoi)
    {
        User? nguoiDung = authService.CurrentUser;
        if (nguoiDung == null)
        {
            thongBaoLoi = "Vui lòng đăng nhập.";
            return false;
        }

        Trip? chuyen = duLieu.Trips.FirstOrDefault(t => t.Id == tripId);
        if (chuyen == null)
        {
            thongBaoLoi = "Không tìm thấy chuyến công tác.";
            return false;
        }

        chuyen.Status = TripStatus.InUse;
        chuyen.Note = $"[Đã khắc phục {DateTime.Now:HH:mm}]: {ghiChu} | {chuyen.Note}";

        duLieu.AddStatusHistory(
            "Trip",
            tripId,
            "NeedSupport",
            "InUse",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            $"Đã khắc phục sự cố, tiếp tục hành trình: {ghiChu}");

        thongBaoLoi = "";
        return true;
    }

    // Các hàm tương thích ngược cũ
    public bool UpdateTripStatus(int tripId, TripStatus trangThaiMoi, string? ghiChu, out string thongBaoLoi)
    {
        if (trangThaiMoi == TripStatus.InTransit || trangThaiMoi == TripStatus.InUse)
        {
            Trip? t = duLieu.Trips.FirstOrDefault(x => x.Id == tripId);
            return HandoverVehicle(tripId, t?.StartKm ?? 0, "100%", "Bàn giao xe xuất phát", out thongBaoLoi);
        }
        else if (trangThaiMoi == TripStatus.NeedSupport)
        {
            return ReportIncident(tripId, ghiChu ?? "Gặp sự cố cần hỗ trợ", out thongBaoLoi);
        }
        else if (trangThaiMoi == TripStatus.Returned || trangThaiMoi == TripStatus.Completed)
        {
            Trip? t = duLieu.Trips.FirstOrDefault(x => x.Id == tripId);
            double endKm = t != null ? t.StartKm + 30 : 0;
            return ReturnVehicle(tripId, endKm, "80%", 5.0, "Xe bình thường", false, "", ghiChu ?? "", out thongBaoLoi);
        }

        thongBaoLoi = "Trạng thái không hợp lệ.";
        return false;
    }

    public bool SettleTrip(int tripId, double endKm, double fuelUsed, string? ghiChu, out string thongBaoLoi)
    {
        Trip? chuyen = duLieu.Trips.FirstOrDefault(t => t.Id == tripId);
        if (chuyen != null && chuyen.Status == TripStatus.Returned && !chuyen.IsSettled)
        {
            if (endKm < chuyen.StartKm)
            {
                thongBaoLoi = $"Số Km kết thúc ({endKm:N0} km) không được nhỏ hơn số Km ban đầu ({chuyen.StartKm:N0} km).";
                return false;
            }

            if (fuelUsed < 0)
            {
                thongBaoLoi = "Nhiên liệu tiêu thụ không được âm.";
                return false;
            }

            User? nguoiDung = authService.CurrentUser;
            chuyen.EndKm = endKm;
            chuyen.FuelUsed = fuelUsed;
            chuyen.IsSettled = true;
            chuyen.Status = TripStatus.Completed;
            chuyen.SettledAt = DateTime.Now;
            chuyen.SettledBy = nguoiDung?.FullName ?? "Quản lý đội xe";
            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                chuyen.Note = string.IsNullOrWhiteSpace(chuyen.Note) ? ghiChu : $"{chuyen.Note} | Quyết toán: {ghiChu}";
            }

            Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == chuyen.VehicleId);
            if (xe != null)
            {
                xe.CurrentKm = endKm;
                if (!chuyen.HasIncident)
                {
                    xe.OperationalStatus = VehicleOperationalStatus.Ready;
                    xe.Status = VehicleStatus.SanSang;
                }
            }

            Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
            if (don != null)
            {
                don.Status = BookingStatus.Completed;
            }

            duLieu.AddStatusHistory(
                "Trip",
                chuyen.Id,
                "Returned",
                "Completed",
                nguoiDung?.OfficerId ?? nguoiDung?.Id ?? 0,
                nguoiDung?.FullName ?? "Hệ thống",
                $"Quyết toán chuyến đi {chuyen.DispatchCode}. Km cuối: {endKm:N0} km, Nhiên liệu tiêu thụ: {fuelUsed:N1}L.");

            GhiNhatKy(nguoiDung?.Id ?? 0, "Quyết toán chuyến đi", $"Hoàn tất quyết toán chuyến đi {chuyen.DispatchCode} cho xe {xe?.LicensePlate}.");

            thongBaoLoi = "";
            return true;
        }

        return ReturnVehicle(tripId, endKm, "80%", fuelUsed, "Bình thường", false, "", ghiChu ?? "", out thongBaoLoi);
    }

    public Booking? GetBooking(int bookingId) => duLieu.Bookings.FirstOrDefault(b => b.Id == bookingId);
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
