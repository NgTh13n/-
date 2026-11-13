using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

public class Trip
{
    public int Id { get; set; }

    // Mã lệnh điều động (Ví dụ: LĐĐ-2026-001)
    public string DispatchCode { get; set; } = "";

    // Mã yêu cầu đăng ký tương ứng
    public int BookingId { get; set; }

    // Phương tiện được phân công
    public int VehicleId { get; set; }

    // Lái xe được phân công
    public int DriverId { get; set; }

    // Cán bộ quản lý đội xe lập lệnh điều động
    public int DispatcherId { get; set; }

    // Thời điểm lập lệnh điều động
    public DateTime DispatchedAt { get; set; } = DateTime.Now;

    // Trạng thái chuyến công tác
    public TripStatus Status { get; set; } = TripStatus.WaitingDeparture;

    // Thời gian xuất phát thực tế
    public DateTime? ActualDeparture { get; set; }

    // Thời gian trở về thực tế
    public DateTime? ActualReturn { get; set; }

    // Số km lúc bắt đầu chuyến đi
    public double StartKm { get; set; }

    // Số km lúc kết thúc chuyến đi
    public double EndKm { get; set; }

    // Nhiên liệu tiêu thụ (lít)
    public double FuelUsed { get; set; }

    // Đã quyết toán chuyến đi hay chưa
    public bool IsSettled { get; set; }

    // Ghi chú phát sinh / Sự cố / Yêu cầu hỗ trợ
    public string Note { get; set; } = "";
}
