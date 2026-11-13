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
    public List<Driver> GetAll()
    {
        List<Driver> ketQua = new List<Driver>();

        foreach (Driver driver in duLieu.Drivers)
        {
            ketQua.Add(SaoChep(driver));
        }

        return ketQua;
    }

    // Thêm lái xe mới
    public bool Add(Driver driver, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền quản lý danh sách lái xe.";
            return false;
        }

        loi = KiemTra(driver, 0);

        if (loi != "")
        {
            return false;
        }

        // Tự động sinh Id tăng dần
        int idMoi = 1;

        foreach (Driver item in duLieu.Drivers)
        {
            if (item.Id >= idMoi)
            {
                idMoi = item.Id + 1;
            }
        }

        driver.Id = idMoi;
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

        if (loi != "")
        {
            return false;
        }

        for (int i = 0; i < duLieu.Drivers.Count; i++)
        {
            if (duLieu.Drivers[i].Id == driver.Id)
            {
                duLieu.Drivers[i] = SaoChep(driver);
                return true;
            }
        }

        loi = "Không tìm thấy thông tin lái xe cần cập nhật.";
        return false;
    }

    // Xóa lái xe
    public bool Delete(int id, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền xóa lái xe.";
            return false;
        }

        // Kiểm tra xem lái xe có đang được phân công chuyến đi chưa hoàn thành không
        foreach (Trip chuyen in duLieu.Trips)
        {
            if (chuyen.DriverId == id)
            {
                bool daXongChuyen = chuyen.Status == TripStatus.Returned && chuyen.IsSettled;

                if (!daXongChuyen)
                {
                    loi = "Không thể xóa lái xe đang tham gia chuyến đi chưa hoàn thành.";
                    return false;
                }
            }
        }

        for (int i = 0; i < duLieu.Drivers.Count; i++)
        {
            if (duLieu.Drivers[i].Id == id)
            {
                duLieu.Drivers.RemoveAt(i);
                loi = "";
                return true;
            }
        }

        loi = "Không tìm thấy thông tin lái xe cần xóa.";
        return false;
    }

    // Kiểm tra dữ liệu hợp lệ
    private string KiemTra(Driver driver, int idCanBoQua)
    {
        driver.FullName = driver.FullName.Trim();
        driver.Phone = driver.Phone.Trim();
        driver.LicenseType = driver.LicenseType.Trim();

        if (string.IsNullOrEmpty(driver.FullName))
        {
            return "Vui lòng nhập họ và tên lái xe.";
        }

        if (string.IsNullOrEmpty(driver.LicenseType))
        {
            return "Vui lòng nhập hạng giấy phép lái xe.";
        }

        // Kiểm tra trùng số điện thoại
        if (!string.IsNullOrEmpty(driver.Phone))
        {
            foreach (Driver item in duLieu.Drivers)
            {
                if (item.Id != idCanBoQua && string.Equals(item.Phone, driver.Phone, StringComparison.OrdinalIgnoreCase))
                {
                    return "Số điện thoại này đã được đăng ký cho một lái xe khác.";
                }
            }
        }

        return "";
    }

    // Tạo bản sao để tránh tham chiếu trực tiếp
    private Driver SaoChep(Driver driver)
    {
        return new Driver
        {
            Id = driver.Id,
            FullName = driver.FullName,
            Phone = driver.Phone,
            LicenseType = driver.LicenseType,
            Status = driver.Status
        };
    }
}
