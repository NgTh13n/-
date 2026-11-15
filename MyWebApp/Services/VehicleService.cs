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
    public List<Vehicle> GetAll() => duLieu.Vehicles.Select(SaoChep).ToList();

    public bool Add(Vehicle vehicle, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền thêm phương tiện.";
            return false;
        }

        loi = KiemTra(vehicle, 0);
        if (loi != "") return false;

        vehicle.Id = duLieu.Vehicles.Count > 0 ? duLieu.Vehicles.Max(x => x.Id) + 1 : 1;
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
        if (loi != "") return false;

        int index = duLieu.Vehicles.FindIndex(v => v.Id == vehicle.Id);
        if (index < 0)
        {
            loi = "Không tìm thấy phương tiện cần sửa.";
            return false;
        }

        duLieu.Vehicles[index] = SaoChep(vehicle);
        return true;
    }

    public bool Delete(int id, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền xóa phương tiện.";
            return false;
        }

        // Chặn xóa xe đang có chuyến đi chưa hoàn tất hoặc chưa quyết toán.
        if (duLieu.Trips.Any(c => c.VehicleId == id && 
            c.Status != TripStatus.Completed && 
            c.Status != TripStatus.Cancelled && 
            !(c.Status == TripStatus.Returned && c.IsSettled)))
        {
            loi = "Không thể xóa phương tiện đang có chuyến đi chưa hoàn thành.";
            return false;
        }

        int index = duLieu.Vehicles.FindIndex(v => v.Id == id);
        if (index < 0)
        {
            loi = "Không tìm thấy phương tiện cần xóa.";
            return false;
        }

        duLieu.Vehicles.RemoveAt(index);
        loi = "";
        return true;
    }

    private string KiemTra(Vehicle vehicle, int idCanBoQua)
    {
        vehicle.VehicleCode = vehicle.VehicleCode.Trim();
        vehicle.LicensePlate = vehicle.LicensePlate.Trim();

        if (vehicle.ManufactureYear > DateTime.Today.Year)
            return "Năm sản xuất không được lớn hơn năm hiện tại.";

        if (vehicle.Seats < 1)
            return "Số chỗ ngồi phải lớn hơn 0.";

        if (vehicle.CurrentKm < 0)
            return "Số km hiện tại không được âm.";

        if (duLieu.Vehicles.Any(xe => xe.Id != idCanBoQua && string.Equals(xe.VehicleCode, vehicle.VehicleCode, StringComparison.OrdinalIgnoreCase)))
            return "Mã phương tiện đã tồn tại.";

        if (duLieu.Vehicles.Any(xe => xe.Id != idCanBoQua && string.Equals(xe.LicensePlate, vehicle.LicensePlate, StringComparison.OrdinalIgnoreCase)))
            return "Biển số đã tồn tại.";

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
            OperationalStatus = vehicle.OperationalStatus,
            RegistrationExpiryDate = vehicle.RegistrationExpiryDate,
            NextMaintenanceDate = vehicle.NextMaintenanceDate,
            Note = vehicle.Note,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt
        };
    }
}
