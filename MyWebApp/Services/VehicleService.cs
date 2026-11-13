using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Services;

public class VehicleService
{
    private readonly MockDataService duLieu;
    private readonly AuthService authService;

    public VehicleService(MockDataService mockDataService, AuthService authService)
    {
        duLieu = mockDataService;
        this.authService = authService;
    }

    // Trả về một bản sao danh sách để form sửa không đổi dữ liệu trước khi bấm Lưu.
    public List<Vehicle> GetAll()
    {
        List<Vehicle> ketQua = new List<Vehicle>();

        foreach (Vehicle vehicle in duLieu.Vehicles)
        {
            ketQua.Add(SaoChep(vehicle));
        }

        return ketQua;
    }

    public bool Add(Vehicle vehicle, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền thêm phương tiện.";
            return false;
        }

        loi = KiemTra(vehicle, 0);

        if (loi != "")
        {
            return false;
        }

        // Cấp mã Id tiếp theo, kể cả khi một xe cũ đã bị xóa.
        int idMoi = 1;

        foreach (Vehicle xe in duLieu.Vehicles)
        {
            if (xe.Id >= idMoi)
            {
                idMoi = xe.Id + 1;
            }
        }

        vehicle.Id = idMoi;
        duLieu.Vehicles.Add(SaoChep(vehicle));

        return true;
    }

    public bool Update(Vehicle vehicle, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền sửa phương tiện.";
            return false;
        }

        loi = KiemTra(vehicle, vehicle.Id);

        if (loi != "")
        {
            return false;
        }

        for (int i = 0; i < duLieu.Vehicles.Count; i++)
        {
            if (duLieu.Vehicles[i].Id == vehicle.Id)
            {
                duLieu.Vehicles[i] = SaoChep(vehicle);
                return true;
            }
        }

        loi = "Không tìm thấy phương tiện cần sửa.";
        return false;
    }

    public bool Delete(int id, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền xóa phương tiện.";
            return false;
        }

        // Chặn xóa xe đang có chuyến đi chưa hoàn thành.
        foreach (Trip chuyen in duLieu.Trips)
        {
            if (chuyen.VehicleId == id)
            {
                bool daHoanThanh = chuyen.Status == TripStatus.Returned && chuyen.IsSettled;

                if (!daHoanThanh)
                {
                    loi = "Không thể xóa phương tiện đang có chuyến đi chưa hoàn thành.";
                    return false;
                }
            }
        }

        for (int i = 0; i < duLieu.Vehicles.Count; i++)
        {
            if (duLieu.Vehicles[i].Id == id)
            {
                duLieu.Vehicles.RemoveAt(i);
                loi = "";
                return true;
            }
        }

        loi = "Không tìm thấy phương tiện cần xóa.";
        return false;
    }

    private string KiemTra(Vehicle vehicle, int idCanBoQua)
    {
        vehicle.VehicleCode = vehicle.VehicleCode.Trim();
        vehicle.LicensePlate = vehicle.LicensePlate.Trim();

        if (vehicle.ManufactureYear > DateTime.Today.Year)
        {
            return "Năm sản xuất không được lớn hơn năm hiện tại.";
        }

        if (vehicle.Seats < 1)
        {
            return "Số chỗ ngồi phải lớn hơn 0.";
        }

        if (vehicle.CurrentKm < 0)
        {
            return "Số km hiện tại không được âm.";
        }

        foreach (Vehicle xe in duLieu.Vehicles)
        {
            if (xe.Id != idCanBoQua &&
                string.Equals(xe.VehicleCode, vehicle.VehicleCode, StringComparison.OrdinalIgnoreCase))
            {
                return "Mã phương tiện đã tồn tại.";
            }

            if (xe.Id != idCanBoQua &&
                string.Equals(xe.LicensePlate, vehicle.LicensePlate, StringComparison.OrdinalIgnoreCase))
            {
                return "Biển số đã tồn tại.";
            }
        }

        return "";
    }

    private Vehicle SaoChep(Vehicle vehicle)
    {
        return new Vehicle
        {
            Id = vehicle.Id,
            VehicleCode = vehicle.VehicleCode,
            LicensePlate = vehicle.LicensePlate,
            VehicleType = vehicle.VehicleType,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Seats = vehicle.Seats,
            ManufactureYear = vehicle.ManufactureYear,
            CurrentKm = vehicle.CurrentKm,
            Color = vehicle.Color,
            ChassisNumber = vehicle.ChassisNumber,
            EngineNumber = vehicle.EngineNumber,
            ReceiveDate = vehicle.ReceiveDate,
            Status = vehicle.Status,
            Note = vehicle.Note
        };
    }
}
