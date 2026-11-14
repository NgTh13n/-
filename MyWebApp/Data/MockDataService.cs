using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Data;

/// <summary>
/// Dịch vụ Singleton quản lý toàn bộ dữ liệu In-Memory trong bộ nhớ RAM của đơn vị.
/// Tuân thủ nguyên tắc không sử dụng CSDL hay file persistence; dữ liệu chia sẻ cho tất cả circuit và reset khi ứng dụng khởi động lại.
/// </summary>
public class MockDataService
{
    private readonly object _lock = new();

    // 1. Danh mục phương tiện công tác (8 phương tiện chuẩn)
    public List<Vehicle> Vehicles { get; set; } = new();

    // 2. Hồ sơ cán bộ / chiến sĩ (12 cán bộ chuẩn)
    public List<Officer> Officers { get; set; } = new();

    // 3. Người dùng / Phiên đăng nhập demo (4 vai trò demo)
    public List<User> Users { get; set; } = new();

    // 4. Danh mục đơn vị / phòng đội trực thuộc (3 đội)
    public List<Department> Departments { get; set; } = new();

    // 5. Danh mục lái xe chuyên trách (4 lái xe)
    public List<Driver> Drivers { get; set; } = new();

    // 6. Yêu cầu sử dụng phương tiện (VehicleRequest / Booking)
    public List<Booking> Bookings { get; set; } = new();

    // 7. Chuyến công tác / Lệnh điều động (Trip)
    public List<Trip> Trips { get; set; } = new();

    // 8. Hồ sơ bảo dưỡng, sửa chữa kỹ thuật
    public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new();

    // 9. Lịch sử chuyển tiếp trạng thái (StatusHistory / Audit trail)
    public List<StatusHistory> StatusHistories { get; set; } = new();

    // 10. Nhật ký hoạt động người dùng
    public List<ActivityLog> ActivityLogs { get; set; } = new();

    public MockDataService()
    {
        ResetSeedData();
    }

    /// <summary>
    /// Khôi phục toàn bộ dữ liệu mẫu theo Đặc tả SRS v1.0
    /// </summary>
    public void ResetSeedData()
    {
        lock (_lock)
        {
            Vehicles = VehicleData.TaoDanhSach();
            Departments = MockDataSeeder.TaoDanhSachDonVi();
            Officers = MockDataSeeder.TaoDanhSachCanBo();
            Users = MockDataSeeder.TaoDanhSachNguoiDung();
            Drivers = MockDataSeeder.TaoDanhSachLaiXe();
            Bookings = MockDataSeeder.TaoDanhSachYeuCau();
            Trips = MockDataSeeder.TaoDanhSachChuyenDi();
            MaintenanceRecords = MockDataSeeder.TaoDanhSachBaoDuong();
            StatusHistories = MockDataSeeder.TaoDanhSachLichSuTrangThai();
            ActivityLogs = MockDataSeeder.TaoDanhSachNhatKy();
        }
    }

    /// <summary>
    /// Tương thích ngược với tên gọi cũ
    /// </summary>
    public void KhoiPhucDuLieuGoc()
    {
        ResetSeedData();
    }

    /// <summary>
    /// Ghi nhận lịch sử chuyển trạng thái
    /// </summary>
    public void AddStatusHistory(string entityType, int entityId, string fromStatus, string toStatus, int? officerId, string officerName, string comment)
    {
        lock (_lock)
        {
            int nextId = StatusHistories.Count > 0 ? StatusHistories.Max(s => s.Id) + 1 : 1;
            StatusHistories.Add(new StatusHistory
            {
                Id = nextId,
                EntityType = entityType,
                EntityId = entityId,
                FromStatus = fromStatus,
                ToStatus = toStatus,
                ChangedAt = DateTime.Now,
                ChangedByOfficerId = officerId,
                ChangedByFullName = officerName,
                Comment = comment
            });
        }
    }
}
