using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Services;

public class DriverService
{
    private readonly MockDataService duLieu;
    private readonly AuthService authService;

    public DriverService(MockDataService mockDataService, AuthService authService)
    {
        duLieu = mockDataService;
        this.authService = authService;
    }

    // Lấy toàn bộ danh sách lái xe (trả về bản sao để tránh form sửa đụng dữ liệu gốc)
    public List<Driver> GetAll() => duLieu.Drivers.Select(SaoChep).ToList();

    // Thêm lái xe mới
    public bool Add(Driver driver, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền quản lý danh sách lái xe.";
            return false;
        }

        loi = KiemTra(driver, 0);
        if (loi != "") return false;

        driver.Id = duLieu.Drivers.Count > 0 ? duLieu.Drivers.Max(x => x.Id) + 1 : 1;
        duLieu.Drivers.Add(SaoChep(driver));
        return true;
    }

    // Cập nhật thông tin lái xe
    public bool Update(Driver driver, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền quản lý danh sách lái xe.";
            return false;
        }

        loi = KiemTra(driver, driver.Id);
        if (loi != "") return false;

        int index = duLieu.Drivers.FindIndex(d => d.Id == driver.Id);
        if (index < 0)
        {
            loi = "Không tìm thấy thông tin lái xe cần cập nhật.";
            return false;
        }

        duLieu.Drivers[index] = SaoChep(driver);
        return true;
    }

    // Xóa lái xe
    public bool Delete(int id, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền xóa lái xe.";
            return false;
        }

        // Kiểm tra xem lái xe có đang được phân công chuyến đi chưa hoàn tất hoặc chưa quyết toán không
        if (duLieu.Trips.Any(c => c.DriverId == id && 
            c.Status != TripStatus.Completed && 
            c.Status != TripStatus.Cancelled && 
            !(c.Status == TripStatus.Returned && c.IsSettled)))
        {
            loi = "Không thể xóa lái xe đang tham gia chuyến đi chưa hoàn thành.";
            return false;
        }

        int index = duLieu.Drivers.FindIndex(d => d.Id == id);
        if (index < 0)
        {
            loi = "Không tìm thấy thông tin lái xe cần xóa.";
            return false;
        }

        duLieu.Drivers.RemoveAt(index);
        loi = "";
        return true;
    }

    // Kiểm tra dữ liệu hợp lệ
    private string KiemTra(Driver driver, int idCanBoQua)
    {
        driver.FullName = driver.FullName.Trim();
        driver.Phone = driver.Phone.Trim();
        driver.LicenseType = driver.LicenseType.Trim();

        if (string.IsNullOrEmpty(driver.FullName))
            return "Vui lòng nhập họ và tên lái xe.";

        if (string.IsNullOrEmpty(driver.LicenseType))
            return "Vui lòng nhập hạng giấy phép lái xe.";

        if (!string.IsNullOrEmpty(driver.Phone) && duLieu.Drivers.Any(d => d.Id != idCanBoQua && string.Equals(d.Phone, driver.Phone, StringComparison.OrdinalIgnoreCase)))
            return "Số điện thoại này đã được đăng ký cho một lái xe khác.";

        return "";
    }

    // Tạo bản sao để tránh tham chiếu trực tiếp
    private Driver SaoChep(Driver driver)
    {
        return new Driver
        {
            Id = driver.Id,
            OfficerId = driver.OfficerId,
            FullName = driver.FullName,
            Phone = driver.Phone,
            LicenseCategory = driver.LicenseCategory,
            LicenseNumberDemo = driver.LicenseNumberDemo,
            LicenseExpiryDate = driver.LicenseExpiryDate,
            IsActive = driver.IsActive,
            Status = driver.Status,
            Notes = driver.Notes
        };
    }
}
