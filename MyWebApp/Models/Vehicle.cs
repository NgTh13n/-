using System.ComponentModel.DataAnnotations;
using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

public class Vehicle
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã phương tiện.")]
    [StringLength(20, ErrorMessage = "Mã phương tiện tối đa 20 ký tự.")]
    public string VehicleCode { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập biển số.")]
    [StringLength(20, ErrorMessage = "Biển số tối đa 20 ký tự.")]
    public string LicensePlate { get; set; } = "";

    public VehicleType VehicleType { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nhãn hiệu.")]
    public string Brand { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập model.")]
    public string Model { get; set; } = "";

    [Range(1, 100, ErrorMessage = "Số chỗ ngồi phải từ 1 đến 100.")]
    public int Seats { get; set; } = 1;

    [Range(1900, 2100, ErrorMessage = "Năm sản xuất không hợp lệ.")]
    public int ManufactureYear { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Số km hiện tại không được âm.")]
    public double CurrentKm { get; set; }

    public string Color { get; set; } = "";
    public string ChassisNumber { get; set; } = "";
    public string EngineNumber { get; set; } = "";
    public DateTime ReceiveDate { get; set; } = DateTime.Today;

    public VehicleStatus Status { get; set; }

    public string Note { get; set; } = "";
}
