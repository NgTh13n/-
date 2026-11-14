using System.ComponentModel.DataAnnotations;
using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    public string Username { get; set; } = "";

    // Mật khẩu lưu dưới dạng văn bản thường vì đây là dữ liệu demo, không có database.
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    public string FullName { get; set; } = "";

    public int DepartmentId { get; set; }

    public UserRole Role { get; set; }

    public string Phone { get; set; } = "";

    // Liên kết với hồ sơ cán bộ (Officer)
    public int? OfficerId { get; set; }
}
