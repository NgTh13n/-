namespace MyWebApp.Models.Enums;

// Tình trạng hoạt động của phương tiện.
public enum VehicleStatus
{
    SanSang,            // Sẵn sàng để điều động
    DangDuocDieuDong,   // Đang trong chuyến đi
    DangBaoDuong,       // Đang bảo dưỡng / sửa chữa
    NgungHoatDong       // Ngừng hoạt động, chờ thanh lý
}
