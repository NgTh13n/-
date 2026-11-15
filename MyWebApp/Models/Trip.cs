using MyWebApp.Models.Enums;

namespace MyWebApp.Models;

/// <summary>
/// Chuyến công tác / Lệnh điều động phương tiện
/// </summary>
public class Trip
{
    public int Id { get; set; }

    // Mã lệnh điều động (Ví dụ: LĐĐ-2026-001)
    public string DispatchCode { get; set; } = "";

    public string TripCode
    {
        get => DispatchCode;
        set => DispatchCode = value;
    }

    // Mã yêu cầu tương ứng
    public int BookingId { get; set; }

    public int RequestId
    {
        get => BookingId;
        set => BookingId = value;
    }

    // Phương tiện được điều động
    public int VehicleId { get; set; }

    // Phương án lái xe: Lái xe đơn vị hoặc Cán bộ tự lái
    public DriverMode DriverMode { get; set; } = DriverMode.AssignedDriver;

    // Lái xe đơn vị (nullable nếu là cán bộ tự lái)
    public int DriverId { get; set; }

    // Cán bộ tự lái (nếu DriverMode = SelfDrive)
    public int? SelfDrivingOfficerId { get; set; }

    // Snapshots: Chụp thông tin tại thời điểm điều phối để lịch sử không bị đổi
    public string DriverNameSnapshot { get; set; } = "";
    public string RequesterNameSnapshot { get; set; } = "";
    public string VehiclePlateSnapshot { get; set; } = "";

    // Lịch trình dự kiến theo yêu cầu
    public DateTime ScheduledStartAt { get; set; } = DateTime.Today.AddHours(8);
    public DateTime ScheduledEndAt { get; set; } = DateTime.Today.AddHours(17);

    // Cán bộ đội xe lập lệnh điều động
    public int DispatcherId { get; set; }

    public int DispatchedByOfficerId
    {
        get => DispatcherId;
        set => DispatcherId = value;
    }

    public DateTime DispatchedAt { get; set; } = DateTime.Now;

    // Trạng thái chuyến
    public TripStatus Status { get; set; } = TripStatus.WaitingDeparture;

    // --- BIÊN BẢN BÀN GIAO XE (Handover) ---
    public DateTime? HandoverAt { get; set; }

    public double StartKm { get; set; }

    public double OdometerStartKm
    {
        get => StartKm;
        set => StartKm = value;
    }

    // Mức nhiên liệu ban đầu: "100%", "80%", "Đầy bình",...
    public string FuelStartLevel { get; set; } = "100%";

    // Tình trạng ngoại quan, phụ kiện xe trước khi bàn giao
    public string ConditionBefore { get; set; } = "Xe bình thường, sạch sẽ, đầy đủ giấy tờ";

    // Thời gian xuất phát thực tế (tương thích code cũ)
    public DateTime? ActualDeparture
    {
        get => HandoverAt;
        set => HandoverAt = value;
    }

    // --- BIÊN BẢN HOÀN TRẢ XE (Return) ---
    public DateTime? ReturnedAt { get; set; }

    public double EndKm { get; set; }

    public double OdometerEndKm
    {
        get => EndKm;
        set => EndKm = value;
    }

    // Mức nhiên liệu khi hoàn trả
    public string FuelEndLevel { get; set; } = "80%";

    // Tình trạng xe khi hoàn trả
    public string ConditionAfter { get; set; } = "Bình thường, không trầy xước";

    // Ghi nhận sự cố / hư hỏng phát sinh
    public bool HasIncident { get; set; } = false;

    public string IncidentNote { get; set; } = "";

    // Thời gian trở về thực tế (tương thích code cũ)
    public DateTime? ActualReturn
    {
        get => ReturnedAt;
        set => ReturnedAt = value;
    }

    // Quãng đường thực tế đã đi (km)
    public double DistanceKm => EndKm >= StartKm ? EndKm - StartKm : 0;

    // Nhiên liệu tiêu thụ (lít)
    public double FuelUsed { get; set; }

    // Đã hoàn tất quyết toán chuyến đi hay chưa
    public bool IsSettled { get; set; }

    public DateTime? SettledAt { get; set; }

    public string SettledBy { get; set; } = "";

    // Ghi chú chung
    public string Note { get; set; } = "";

    public string Notes
    {
        get => Note;
        set => Note = value;
    }

    // Tự động kiểm tra trạng thái Quá hạn (Overdue)
    public bool IsOverdue =>
        (Status == TripStatus.InTransit || Status == TripStatus.InUse) &&
        ScheduledEndAt < DateTime.Now;
}
