using System.ComponentModel.DataAnnotations;
using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

/// <summary>
/// Yêu cầu sử dụng phương tiện công tác (VehicleRequest / Booking)
/// </summary>
public class Booking
{
    public int Id { get; set; }

    public string BookingCode { get; set; } = "";

    // Alias RequestCode
    public string RequestCode
    {
        get => BookingCode;
        set => BookingCode = value;
    }

    public int RequesterId { get; set; }

    // Alias RequesterOfficerId
    public int RequesterOfficerId
    {
        get => RequesterId;
        set => RequesterId = value;
    }

    public int DepartmentId { get; set; }

    // Bản ghi tên bộ phận tại thời điểm đăng ký (đảm bảo không bị ảnh hưởng nếu đổi tên sau này)
    public string DepartmentSnapshot { get; set; } = "";

    public string WorkType { get; set; } = "Công tác địa bàn";

    // Danh mục mục đích: Điều tra phá án, Tuần tra kiểm soát, Họp nghiệp vụ, v.v.
    public string PurposeCategory
    {
        get => WorkType;
        set => WorkType = value;
    }

    [Required(ErrorMessage = "Vui lòng nhập mục đích sử dụng.")]
    public string Purpose { get; set; } = "";

    // Nội dung / nhiệm vụ ngắn (10 - 500 ký tự) theo đặc tả v1.0
    public string TaskDescription { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa điểm xuất phát.")]
    public string Departure { get; set; } = "Trụ sở đơn vị";

    [Required(ErrorMessage = "Vui lòng nhập địa điểm đến.")]
    public string Destination { get; set; } = "";

    public DateTime DepartureTime { get; set; } = DateTime.Today.AddHours(8);

    // Alias RequestedStartAt
    public DateTime RequestedStartAt
    {
        get => DepartureTime;
        set => DepartureTime = value;
    }

    public DateTime ReturnTime { get; set; } = DateTime.Today.AddHours(17);

    // Alias RequestedEndAt
    public DateTime RequestedEndAt
    {
        get => ReturnTime;
        set => ReturnTime = value;
    }

    [Range(1, 100, ErrorMessage = "Số người phải từ 1 đến 100.")]
    public int PassengerCount { get; set; } = 1;

    [Required(ErrorMessage = "Vui lòng chọn loại phương tiện mong muốn.")]
    public string RequestedVehicleType { get; set; } = "Ô tô";

    // Phương án lái xe mong muốn: Lái xe đơn vị (AssignedDriver) hoặc Cán bộ tự lái (SelfDrive)
    public DriverMode PreferredDriverMode { get; set; } = DriverMode.AssignedDriver;

    public BookingStatus Status { get; set; } = BookingStatus.Draft;

    // Lý do từ chối (khi lãnh đạo từ chối)
    public string RejectionReason { get; set; } = "";

    // Lý do yêu cầu chỉnh sửa / bổ sung thông tin (khi lãnh đạo yêu cầu)
    public string NeedAdjustmentReason { get; set; } = "";

    // Thông tin phê duyệt
    public int? ApprovalOfficerId { get; set; }
    public DateTime? ApprovalAt { get; set; }
    public string ApprovalComment { get; set; } = "";

    // Lý do hủy yêu cầu (nếu có)
    public string CancellationReason { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? SubmittedAt { get; set; }

    public string Notes { get; set; } = "";
}
