using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Data;

public class MockDataService
{
    // Danh sách này đóng vai trò như dữ liệu chung của đơn vị trong RAM.
    public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public List<User> Users { get; set; } = new List<User>();

    public List<Department> Departments { get; set; } = new List<Department>();

    public List<Driver> Drivers { get; set; } = new List<Driver>();

    public List<Booking> Bookings { get; set; } = new List<Booking>();

    public List<Trip> Trips { get; set; } = new List<Trip>();

    public List<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public MockDataService()
    {
        KhoiPhucDuLieuGoc();
    }

    // Phương thức khôi phục lại dữ liệu mẫu ban đầu theo yêu cầu của đặc tả
    public void KhoiPhucDuLieuGoc()
    {
        Vehicles = VehicleData.TaoDanhSach();
        Users = MockDataSeeder.TaoDanhSachNguoiDung();
        Departments = MockDataSeeder.TaoDanhSachDonVi();
        Drivers = MockDataSeeder.TaoDanhSachLaiXe();
        Bookings = MockDataSeeder.TaoDanhSachYeuCau();
        Trips = MockDataSeeder.TaoDanhSachChuyenDi();
        ActivityLogs = MockDataSeeder.TaoDanhSachNhatKy();
    }
}
