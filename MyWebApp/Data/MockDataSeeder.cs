using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Data;

public static class MockDataSeeder
{
    public static List<Department> TaoDanhSachDonVi()
    {
        List<Department> danhSach = new List<Department>();

        danhSach.Add(new Department
        {
            Id = 1,
            Name = "Công an phường Minh Khai",
            Description = "Đơn vị demo để thử nghiệm quản lý phương tiện."
        });

        return danhSach;
    }

    public static List<User> TaoDanhSachNguoiDung()
    {
        List<User> danhSach = new List<User>();

        danhSach.Add(new User
        {
            Id = 1,
            Username = "canbo",
            Password = "123",
            FullName = "Nguyễn Minh An",
            DepartmentId = 1,
            Role = UserRole.Officer,
            Phone = "0900000001"
        });

        danhSach.Add(new User
        {
            Id = 2,
            Username = "lanhdao",
            Password = "123",
            FullName = "Trần Quốc Bình",
            DepartmentId = 1,
            Role = UserRole.Leader,
            Phone = "0900000002"
        });

        danhSach.Add(new User
        {
            Id = 3,
            Username = "doixe",
            Password = "123",
            FullName = "Lê Hoàng Nam",
            DepartmentId = 1,
            Role = UserRole.FleetManager,
            Phone = "0900000003"
        });

        danhSach.Add(new User
        {
            Id = 4,
            Username = "admin",
            Password = "123",
            FullName = "Phạm Thu Hà",
            DepartmentId = 1,
            Role = UserRole.Administrator,
            Phone = "0900000004"
        });

        return danhSach;
    }

    public static List<Driver> TaoDanhSachLaiXe()
    {
        List<Driver> danhSach = new List<Driver>();

        danhSach.Add(new Driver
        {
            Id = 1,
            FullName = "Vũ Đức Long",
            Phone = "0910000001",
            LicenseType = "B2",
            Status = DriverStatus.Active
        });

        danhSach.Add(new Driver
        {
            Id = 2,
            FullName = "Đỗ Văn Hùng",
            Phone = "0910000002",
            LicenseType = "C",
            Status = DriverStatus.Active
        });

        danhSach.Add(new Driver
        {
            Id = 3,
            FullName = "Ngô Quang Huy",
            Phone = "0910000003",
            LicenseType = "B2",
            Status = DriverStatus.Active
        });

        danhSach.Add(new Driver
        {
            Id = 4,
            FullName = "Bùi Thanh Tùng",
            Phone = "0910000004",
            LicenseType = "C",
            Status = DriverStatus.OnLeave
        });

        danhSach.Add(new Driver
        {
            Id = 5,
            FullName = "Đặng Văn Sơn",
            Phone = "0910000005",
            LicenseType = "B2",
            Status = DriverStatus.Active
        });

        return danhSach;
    }

    public static List<Booking> TaoDanhSachYeuCau()
    {
        DateTime homNay = DateTime.Today;
        List<Booking> danhSach = new List<Booking>();

        danhSach.Add(new Booking
        {
            Id = 1,
            BookingCode = $"YC-{homNay.Year}-001",
            RequesterId = 1,
            DepartmentId = 1,
            WorkType = "Công tác địa bàn",
            Purpose = "Làm việc tại địa bàn được phân công",
            Departure = "Trụ sở đơn vị",
            Destination = "Phường Minh Khai",
            DepartureTime = homNay.AddDays(1).AddHours(8),
            ReturnTime = homNay.AddDays(1).AddHours(11),
            PassengerCount = 3,
            RequestedVehicleType = "Ô tô",
            Status = BookingStatus.PendingApproval
        });

        danhSach.Add(new Booking
        {
            Id = 2,
            BookingCode = $"YC-{homNay.Year}-002",
            RequesterId = 1,
            DepartmentId = 1,
            WorkType = "Họp nghiệp vụ",
            Purpose = "Tham dự cuộc họp nghiệp vụ",
            Departure = "Trụ sở đơn vị",
            Destination = "Trụ sở Công an quận",
            DepartureTime = homNay.AddDays(2).AddHours(8),
            ReturnTime = homNay.AddDays(2).AddHours(12),
            PassengerCount = 4,
            RequestedVehicleType = "Ô tô",
            Status = BookingStatus.PendingDispatch
        });

        danhSach.Add(new Booking
        {
            Id = 3,
            BookingCode = $"YC-{homNay.Year}-003",
            RequesterId = 1,
            DepartmentId = 1,
            WorkType = "Công tác địa bàn",
            Purpose = "Kiểm tra địa điểm công tác",
            Departure = "Trụ sở đơn vị",
            Destination = "Phường Minh Khai",
            DepartureTime = homNay.AddDays(-2).AddHours(9),
            ReturnTime = homNay.AddDays(-2).AddHours(11),
            PassengerCount = 2,
            RequestedVehicleType = "Xe máy",
            Status = BookingStatus.Rejected,
            RejectionReason = "Vui lòng bổ sung thông tin về lịch trình."
        });

        danhSach.Add(new Booking
        {
            Id = 4,
            BookingCode = $"YC-{homNay.Year}-004",
            RequesterId = 1,
            DepartmentId = 1,
            WorkType = "Công tác địa bàn",
            Purpose = "Phối hợp xử lý công việc tại địa bàn",
            Departure = "Trụ sở đơn vị",
            Destination = "Khu vực phía Bắc phường",
            DepartureTime = homNay.AddHours(7),
            ReturnTime = homNay.AddHours(13),
            PassengerCount = 5,
            RequestedVehicleType = "Ô tô",
            Status = BookingStatus.InProgress
        });

        danhSach.Add(new Booking
        {
            Id = 5,
            BookingCode = $"YC-{homNay.Year}-005",
            RequesterId = 1,
            DepartmentId = 1,
            WorkType = "Chuyển hồ sơ",
            Purpose = "Chuyển hồ sơ công tác đến đơn vị phối hợp",
            Departure = "Trụ sở đơn vị",
            Destination = "Trụ sở Công an quận",
            DepartureTime = homNay.AddDays(-1).AddHours(10),
            ReturnTime = homNay.AddDays(-1).AddHours(15),
            PassengerCount = 2,
            RequestedVehicleType = "Ô tô",
            Status = BookingStatus.PendingSettlement
        });

        return danhSach;
    }

    public static List<Trip> TaoDanhSachChuyenDi()
    {
        DateTime homNay = DateTime.Today;
        List<Trip> danhSach = new List<Trip>();

        danhSach.Add(new Trip
        {
            Id = 1,
            DispatchCode = $"LĐĐ-{homNay.Year}-001",
            DispatcherId = 3,
            DispatchedAt = homNay.AddHours(6).AddMinutes(30),
            BookingId = 4,
            VehicleId = 4,
            DriverId = 1,
            Status = TripStatus.InTransit,
            ActualDeparture = homNay.AddHours(7),
            StartKm = 17800,
            EndKm = 17800,
            FuelUsed = 0,
            IsSettled = false
        });

        danhSach.Add(new Trip
        {
            Id = 2,
            DispatchCode = $"LĐĐ-{homNay.Year}-002",
            DispatcherId = 3,
            DispatchedAt = homNay.AddDays(-1).AddHours(9),
            BookingId = 5,
            VehicleId = 1,
            DriverId = 2,
            Status = TripStatus.Returned,
            ActualDeparture = homNay.AddDays(-1).AddHours(10),
            ActualReturn = homNay.AddDays(-1).AddHours(15),
            StartKm = 28450,
            EndKm = 28580,
            FuelUsed = 20,
            IsSettled = false
        });

        return danhSach;
    }

    public static List<ActivityLog> TaoDanhSachNhatKy()
    {
        DateTime homNay = DateTime.Today;
        List<ActivityLog> danhSach = new List<ActivityLog>();

        danhSach.Add(new ActivityLog
        {
            Id = 1,
            UserId = 1,
            Action = "Tạo yêu cầu",
            Description = $"Đã tạo yêu cầu YC-{homNay.Year}-001.",
            CreatedAt = homNay.AddHours(8)
        });

        danhSach.Add(new ActivityLog
        {
            Id = 2,
            UserId = 2,
            Action = "Từ chối yêu cầu",
            Description = $"Đã từ chối yêu cầu YC-{homNay.Year}-003 và ghi lý do.",
            CreatedAt = homNay.AddDays(-2).AddHours(8)
        });

        danhSach.Add(new ActivityLog
        {
            Id = 3,
            UserId = 3,
            Action = "Điều động phương tiện",
            Description = $"Đã phân công phương tiện PT-004 cho YC-{homNay.Year}-004.",
            CreatedAt = homNay.AddHours(7)
        });

        return danhSach;
    }
}
