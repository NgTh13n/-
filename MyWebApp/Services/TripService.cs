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
        List<Trip> ketQua = new List<Trip>();
        User? nguoiDung = authService.CurrentUser;

        if (nguoiDung == null)
        {
            return ketQua;
        }

        bool laCanBo = nguoiDung.Role == UserRole.Officer;

        foreach (Trip t in duLieu.Trips)
        {
            if (laCanBo)
            {
                Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == t.BookingId);
                if (don != null && don.RequesterId == nguoiDung.Id)
                {
                    ketQua.Add(SaoChepTrip(t));
                }
            }
            else
            {
                ketQua.Add(SaoChepTrip(t));
            }
        }

        return ketQua;
    }

    // Lấy danh sách các chuyến đi cần theo dõi (chưa quyết toán)
    public List<Trip> GetActiveTrips()
    {
        List<Trip> ketQua = new List<Trip>();

        foreach (Trip t in duLieu.Trips)
        {
            if (!t.IsSettled)
            {
                ketQua.Add(SaoChepTrip(t));
            }
        }

        return ketQua;
    }

    // Lấy danh sách các chuyến đã trở về và đang chờ quyết toán
    public List<Trip> GetPendingSettlementTrips()
    {
        List<Trip> ketQua = new List<Trip>();

        foreach (Trip t in duLieu.Trips)
        {
            if (t.Status == TripStatus.Returned && !t.IsSettled)
            {
                ketQua.Add(SaoChepTrip(t));
            }
        }

        return ketQua;
    }

    // Lấy danh sách các chuyến đã hoàn tất quyết toán
    public List<Trip> GetSettledTrips()
    {
        List<Trip> ketQua = new List<Trip>();

        foreach (Trip t in duLieu.Trips)
        {
            if (t.IsSettled)
            {
                ketQua.Add(SaoChepTrip(t));
            }
        }

        return ketQua;
    }

    /// <summary>
    /// CẬP NHẬT TRẠNG THÁI CHUYẾN ĐI (Module 5: Theo dõi chuyến công tác)
    /// Chờ xuất phát -> Đang di chuyển -> Cần hỗ trợ -> Đã trở về
    /// </summary>
    public bool UpdateTripStatus(int tripId, TripStatus trangThaiMoi, string? ghiChu, out string thongBaoLoi)
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

        if (chuyen.IsSettled)
        {
            thongBaoLoi = "Chuyến đi đã hoàn tất quyết toán, không thể thay đổi trạng thái.";
            return false;
        }

        Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == chuyen.VehicleId);

        // 1. Chuyển sang "Đang di chuyển" (Xuất phát)
        if (trangThaiMoi == TripStatus.InTransit)
        {
            if (!chuyen.ActualDeparture.HasValue)
            {
                chuyen.ActualDeparture = DateTime.Now;
            }

            if (don != null)
            {
                don.Status = BookingStatus.InProgress;
            }

            chuyen.Status = TripStatus.InTransit;

            GhiNhatKy(nguoiDung.Id, "Xuất phát chuyến công tác",
                $"Chuyến đi {chuyen.DispatchCode} (Xe {xe?.LicensePlate}) đã xuất phát lúc {chuyen.ActualDeparture:dd/MM/yyyy HH:mm}.");
        }
        // 2. Chuyển sang "Cần hỗ trợ" (Gặp sự cố trên đường)
        else if (trangThaiMoi == TripStatus.NeedSupport)
        {
            chuyen.Status = TripStatus.NeedSupport;
            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                chuyen.Note = $"[Sự cố {DateTime.Now:HH:mm dd/MM}]: {ghiChu}";
            }

            GhiNhatKy(nguoiDung.Id, "Yêu cầu hỗ trợ khẩn cấp",
                $"Chuyến đi {chuyen.DispatchCode} (Xe {xe?.LicensePlate}) báo cần hỗ trợ: {ghiChu}");
        }
        // 3. Chuyển sang "Đã trở về" (Kết thúc hành trình, chờ quyết toán)
        else if (trangThaiMoi == TripStatus.Returned)
        {
            chuyen.ActualReturn = DateTime.Now;
            chuyen.Status = TripStatus.Returned;

            if (don != null)
            {
                don.Status = BookingStatus.PendingSettlement;
            }

            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                chuyen.Note += $" | Ghi chú về: {ghiChu}";
            }

            GhiNhatKy(nguoiDung.Id, "Kết thúc chuyến đi",
                $"Chuyến đi {chuyen.DispatchCode} (Xe {xe?.LicensePlate}) đã trở về đơn vị an toàn lúc {chuyen.ActualReturn:dd/MM/yyyy HH:mm}. Chuyển sang chờ quyết toán.");
        }
        else
        {
            chuyen.Status = trangThaiMoi;
        }

        thongBaoLoi = "";
        return true;
    }

    /// <summary>
    /// QUYẾT TOÁN CHUYẾN CÔNG TÁC (Module 6: Quyết toán chuyến công tác)
    /// Áp dụng các quy tắc 11, 12, 13, 14
    /// </summary>
    public bool SettleTrip(int tripId, double endKm, double fuelUsed, string ghiChu, out string thongBaoLoi)
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
            thongBaoLoi = "Không tìm thấy thông tin chuyến công tác cần quyết toán.";
            return false;
        }

        // Quy tắc 14: Một chuyến chỉ được quyết toán một lần
        if (chuyen.IsSettled)
        {
            thongBaoLoi = "Chuyến đi này đã được quyết toán trước đó, không thể quyết toán lại.";
            return false;
        }

        // Quy tắc 11: Chỉ chuyến đi đã trở về mới được quyết toán
        if (chuyen.Status != TripStatus.Returned)
        {
            thongBaoLoi = "Chỉ có thể quyết toán khi phương tiện đã trở về đơn vị.";
            return false;
        }

        // Quy tắc 12: Số km kết thúc phải lớn hơn hoặc bằng số km ban đầu
        if (endKm < chuyen.StartKm)
        {
            thongBaoLoi = $"Số Km kết thúc ({endKm:N0} km) không được nhỏ hơn số Km ban đầu ({chuyen.StartKm:N0} km).";
            return false;
        }

        // Quy tắc 13: Nhiên liệu tiêu thụ không được âm
        if (fuelUsed < 0)
        {
            thongBaoLoi = "Nhiên liệu tiêu thụ không được nhỏ hơn 0.";
            return false;
        }

        double quangDuong = endKm - chuyen.StartKm;

        // Cập nhật thông tin Trip
        chuyen.EndKm = endKm;
        chuyen.FuelUsed = fuelUsed;
        chuyen.IsSettled = true;

        if (!string.IsNullOrWhiteSpace(ghiChu))
        {
            chuyen.Note = string.IsNullOrWhiteSpace(chuyen.Note) 
                ? $"[Quyết toán]: {ghiChu}" 
                : $"{chuyen.Note} | [Quyết toán]: {ghiChu}";
        }

        // Cập nhật trạng thái Booking -> Hoàn thành
        Booking? don = duLieu.Bookings.FirstOrDefault(b => b.Id == chuyen.BookingId);
        if (don != null)
        {
            don.Status = BookingStatus.Completed;
        }

        // Cập nhật Phương tiện: Cập nhật chỉ số Km mới và giải phóng trạng thái về "Sẵn sàng"
        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == chuyen.VehicleId);
        if (xe != null)
        {
            xe.CurrentKm = endKm;
            xe.Status = VehicleStatus.SanSang;
        }

        // Ghi nhật ký hoạt động
        GhiNhatKy(nguoiDung.Id, "Quyết toán chuyến công tác",
            $"Cán bộ {nguoiDung.FullName} đã quyết toán chuyến {chuyen.DispatchCode} (Xe {xe?.LicensePlate}). Quãng đường: {quangDuong:N0} km, Nhiên liệu: {fuelUsed:N1} lít. Đưa xe về trạng thái sẵn sàng.");

        thongBaoLoi = "";
        return true;
    }

    public Booking? GetBooking(int bookingId)
    {
        return duLieu.Bookings.FirstOrDefault(b => b.Id == bookingId);
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
            IsSettled = t.IsSettled,
            Note = t.Note
        };
    }
}
