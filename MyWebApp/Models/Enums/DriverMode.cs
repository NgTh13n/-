namespace MyWebApp.Models.Enums;

/// <summary>
/// Phương án điều khiển phương tiện theo đặc tả CAND:
/// - AssignedDriver: Phân công Lái xe đơn vị
/// - SelfDrive: Cán bộ tự lái (yêu cầu cán bộ đủ điều kiện CanSelfDrive = true)
/// </summary>
public enum DriverMode
{
    AssignedDriver, // Lái xe đơn vị
    SelfDrive       // Cán bộ tự lái
}
