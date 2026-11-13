using System.ComponentModel.DataAnnotations;
using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

public class Booking
{
    public int Id { get; set; }

    public string BookingCode { get; set; } = "";

    public int RequesterId { get; set; }

    public int DepartmentId { get; set; }

    public string WorkType { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mục đích sử dụng.")]
    public string Purpose { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa điểm xuất phát.")]
    public string Departure { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa điểm đến.")]
    public string Destination { get; set; } = "";

    public DateTime DepartureTime { get; set; } = DateTime.Today.AddHours(8);

    public DateTime ReturnTime { get; set; } = DateTime.Today.AddHours(17);

    [Range(1, 100, ErrorMessage = "Số người phải từ 1 đến 100.")]
    public int PassengerCount { get; set; } = 1;

    [Required(ErrorMessage = "Vui lòng chọn loại phương tiện mong muốn.")]
    public string RequestedVehicleType { get; set; } = "Ô tô";

    public BookingStatus Status { get; set; } = BookingStatus.Draft;

    public string RejectionReason { get; set; } = "";
}
