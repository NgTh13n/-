using System.ComponentModel.DataAnnotations;
using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

/// <summary>
/// Hồ sơ phương tiện công tác của đơn vị Công an
/// </summary>
public class Vehicle
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã phương tiện.")]
    [StringLength(20, ErrorMessage = "Mã phương tiện tối đa 20 ký tự.")]
    public string VehicleCode { get; set; } = "";

    // Alias Code
    public string Code
    {
        get => VehicleCode;
        set => VehicleCode = value;
    }

    [Required(ErrorMessage = "Vui lòng nhập biển số.")]
    [StringLength(20, ErrorMessage = "Biển số tối đa 20 ký tự.")]
    public string LicensePlate { get; set; } = "";

    // Alias PlateNumber
    public string PlateNumber
    {
        get => LicensePlate;
        set => LicensePlate = value;
    }

    public VehicleType VehicleType { get; set; } = VehicleType.OTo;

    [Required(ErrorMessage = "Vui lòng nhập nhãn hiệu.")]
    public string Brand { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập model.")]
    public string Model { get; set; } = "";

    [Range(1, 100, ErrorMessage = "Số chỗ ngồi phải từ 1 đến 100.")]
    public int Seats { get; set; } = 4;

    public int SeatCount
    {
        get => Seats;
        set => Seats = value;
    }

    [Range(1900, 2100, ErrorMessage = "Năm sản xuất không hợp lệ.")]
    public int ManufactureYear { get; set; } = DateTime.Today.Year;

    [Range(0, double.MaxValue, ErrorMessage = "Số km hiện tại không được âm.")]
    public double CurrentKm { get; set; }

    // Alias CurrentOdometerKm
    public double CurrentOdometerKm
    {
        get => CurrentKm;
        set => CurrentKm = value;
    }

    public string Color { get; set; } = "Trắng";
    public string ChassisNumber { get; set; } = "";
    public string EngineNumber { get; set; } = "";
    public DateTime ReceiveDate { get; set; } = DateTime.Today;

    // Trạng thái vận hành theo đặc tả v1.0: Ready, Maintenance, Inactive
    public VehicleOperationalStatus OperationalStatus { get; set; } = VehicleOperationalStatus.Ready;

    // Hạn kiểm định / đăng kiểm phương tiện
    public DateTime? RegistrationExpiryDate { get; set; }

    // Hạn bảo dưỡng kế tiếp dự kiến
    public DateTime? NextMaintenanceDate { get; set; }

    // Trạng thái cũ (đảm bảo tương thích ngược 100% với các trang hiện có)
    public VehicleStatus Status
    {
        get
        {
            return OperationalStatus switch
            {
                VehicleOperationalStatus.Maintenance => VehicleStatus.DangBaoDuong,
                VehicleOperationalStatus.Inactive => VehicleStatus.NgungHoatDong,
                _ => VehicleStatus.SanSang
            };
        }
        set
        {
            OperationalStatus = value switch
            {
                VehicleStatus.DangBaoDuong => VehicleOperationalStatus.Maintenance,
                VehicleStatus.NgungHoatDong => VehicleOperationalStatus.Inactive,
                _ => VehicleOperationalStatus.Ready
            };
        }
    }

    public string Note { get; set; } = "";

    public string Notes
    {
        get => Note;
        set => Note = value;
    }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
