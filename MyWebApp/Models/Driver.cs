using System.ComponentModel.DataAnnotations;
using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

public class Driver
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên lái xe.")]
    public string FullName { get; set; } = "";

    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập loại giấy phép lái xe.")]
    public string LicenseType { get; set; } = "";

    public DriverStatus Status { get; set; }
}
