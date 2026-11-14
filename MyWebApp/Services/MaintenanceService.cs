using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Services;

/// <summary>
/// Dịch vụ quản lý hồ sơ bảo dưỡng, sửa chữa kỹ thuật và kiểm định phương tiện
/// </summary>
public class MaintenanceService
{
    private readonly MockDataService duLieu;
    private readonly AuthService authService;

    public MaintenanceService(MockDataService mockDataService, AuthService authService)
    {
        duLieu = mockDataService;
        this.authService = authService;
    }

    public List<MaintenanceRecord> GetAll() =>
        duLieu.MaintenanceRecords.Select(SaoChep).ToList();

    public List<MaintenanceRecord> GetActiveRecords() =>
        duLieu.MaintenanceRecords.Where(item => item.Status == MaintenanceStatus.Planned || item.Status == MaintenanceStatus.InProgress)
                                 .Select(SaoChep).ToList();

    public MaintenanceRecord? GetById(int id) =>
        duLieu.MaintenanceRecords.FirstOrDefault(x => x.Id == id) is { } item ? SaoChep(item) : null;

    /// <summary>
    /// Tạo mới phiếu bảo dưỡng; tự động cập nhật trạng thái phương tiện sang "Đang bảo dưỡng" (Maintenance) nếu cần
    /// </summary>
    public bool Add(MaintenanceRecord record, bool chuyenTrangThaiXeSangBaoDuong, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền lập hồ sơ bảo dưỡng phương tiện.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(record.Description))
        {
            loi = "Vui lòng nhập nội dung bảo dưỡng / sửa chữa.";
            return false;
        }

        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == record.VehicleId);
        if (xe == null)
        {
            loi = "Không tìm thấy phương tiện cần bảo dưỡng.";
            return false;
        }

        // Chặn đưa vào bảo dưỡng nếu xe đang có chuyến đi chưa hoàn thành
        if (duLieu.Trips.FirstOrDefault(c => c.VehicleId == xe.Id && (c.Status == TripStatus.InTransit || c.Status == TripStatus.InUse)) is { } chuyen)
        {
            loi = $"Phương tiện {xe.LicensePlate} đang thực hiện chuyến công tác {chuyen.DispatchCode}, không thể đưa vào bảo dưỡng.";
            return false;
        }

        int idMoi = duLieu.MaintenanceRecords.Count > 0 ? duLieu.MaintenanceRecords.Max(x => x.Id) + 1 : 1;
        record.Id = idMoi;
        duLieu.MaintenanceRecords.Add(SaoChep(record));

        if (chuyenTrangThaiXeSangBaoDuong)
        {
            xe.OperationalStatus = VehicleOperationalStatus.Maintenance;
            duLieu.AddStatusHistory("Vehicle", xe.Id, "Ready", "Maintenance", authService.CurrentUser?.Id, authService.CurrentUser?.FullName ?? "Hệ thống", $"Đưa xe vào bảo dưỡng theo phiếu #{idMoi}: {record.Description}");
        }

        loi = "";
        return true;
    }

    /// <summary>
    /// Nghiệm thu hoàn tất bảo dưỡng, đưa phương tiện về trạng thái Sẵn sàng (Ready)
    /// </summary>
    public bool CompleteMaintenance(int recordId, DateTime ngayHoanThanh, string? ghiChuNghiemThu, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền nghiệm thu bảo dưỡng.";
            return false;
        }

        MaintenanceRecord? record = duLieu.MaintenanceRecords.FirstOrDefault(r => r.Id == recordId);
        if (record == null)
        {
            loi = "Không tìm thấy hồ sơ bảo dưỡng.";
            return false;
        }

        if (record.Status == MaintenanceStatus.Completed)
        {
            loi = "Hồ sơ này đã hoàn tất nghiệm thu trước đó.";
            return false;
        }

        record.Status = MaintenanceStatus.Completed;
        record.CompletedAt = ngayHoanThanh;
        if (!string.IsNullOrWhiteSpace(ghiChuNghiemThu))
        {
            record.Notes = string.IsNullOrWhiteSpace(record.Notes)
                ? $"[Nghiệm thu {ngayHoanThanh:dd/MM/yyyy}]: {ghiChuNghiemThu}"
                : $"{record.Notes} | [Nghiệm thu]: {ghiChuNghiemThu}";
        }

        // Đưa xe về trạng thái Sẵn sàng nếu không còn hồ sơ bảo dưỡng nào khác đang dở dang
        Vehicle? xe = duLieu.Vehicles.FirstOrDefault(v => v.Id == record.VehicleId);
        if (xe != null)
        {
            bool conBaoDuongKhac = duLieu.MaintenanceRecords.Any(r => r.VehicleId == xe.Id && r.Id != recordId && (r.Status == MaintenanceStatus.Planned || r.Status == MaintenanceStatus.InProgress));
            if (!conBaoDuongKhac)
            {
                xe.OperationalStatus = VehicleOperationalStatus.Ready;
                xe.NextMaintenanceDate = DateTime.Today.AddMonths(6); // Gia hạn lần bảo dưỡng kế tiếp
                duLieu.AddStatusHistory("Vehicle", xe.Id, "Maintenance", "Ready", authService.CurrentUser?.Id, authService.CurrentUser?.FullName ?? "Hệ thống", $"Nghiệm thu bảo dưỡng hoàn tất phiếu #{recordId}. Đưa xe về sẵn sàng.");
            }
        }

        loi = "";
        return true;
    }

    private MaintenanceRecord SaoChep(MaintenanceRecord m)
    {
        return new MaintenanceRecord
        {
            Id = m.Id,
            VehicleId = m.VehicleId,
            MaintenanceType = m.MaintenanceType,
            Description = m.Description,
            PlannedAt = m.PlannedAt,
            CompletedAt = m.CompletedAt,
            Status = m.Status,
            OdometerKm = m.OdometerKm,
            GarageName = m.GarageName,
            Cost = m.Cost,
            Notes = m.Notes
        };
    }
}
