using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Data;

public static class VehicleData
{
    /// <summary>
    /// Tạo danh sách 8 phương tiện công tác mẫu theo đúng chuẩn Đặc tả Mục 11.1:
    /// - 5 ô tô (4–7 chỗ), 2 xe bán tải/chuyên dụng, 1 xe máy công vụ
    /// - Trạng thái: 5 sẵn sàng, 1 đang bảo dưỡng, 1 ngừng hoạt động, 1 có chuyến đang sử dụng
    /// - Biển số giả lập chuẩn công vụ CAND (80A, 80B, 80M1)
    /// </summary>
    public static List<Vehicle> TaoDanhSach()
    {
        DateTime homNay = DateTime.Today;

        return new List<Vehicle>
        {
            // 1. Ô tô 7 chỗ Fortuner - Sẵn sàng
            new Vehicle
            {
                Id = 1,
                VehicleCode = "PT-001",
                LicensePlate = "80A-012.34",
                VehicleType = VehicleType.OTo,
                Brand = "Toyota",
                Model = "Fortuner 2.4G",
                Seats = 7,
                ManufactureYear = 2022,
                CurrentKm = 28500,
                Color = "Trắng",
                ChassisNumber = "CAND-CH-001",
                EngineNumber = "CAND-EN-001",
                ReceiveDate = new DateTime(2022, 1, 15),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(8),
                NextMaintenanceDate = homNay.AddMonths(3),
                Note = "Xe công vụ phục vụ công tác chỉ huy, tuần tra địa bàn."
            },

            // 2. Ô tô 7 chỗ Santa Fe - Sẵn sàng
            new Vehicle
            {
                Id = 2,
                VehicleCode = "PT-002",
                LicensePlate = "80A-023.45",
                VehicleType = VehicleType.OTo,
                Brand = "Hyundai",
                Model = "Santa Fe Premium",
                Seats = 7,
                ManufactureYear = 2023,
                CurrentKm = 18200,
                Color = "Đen",
                ChassisNumber = "CAND-CH-002",
                EngineNumber = "CAND-EN-002",
                ReceiveDate = new DateTime(2023, 3, 20),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(12),
                NextMaintenanceDate = homNay.AddMonths(4),
                Note = "Xe phục vụ công tác đối ngoại và đón tiếp đoàn công tác."
            },

            // 3. Ô tô 7 chỗ Xpander - Có chuyến đang sử dụng (InUse)
            new Vehicle
            {
                Id = 3,
                VehicleCode = "PT-003",
                LicensePlate = "80A-034.56",
                VehicleType = VehicleType.OTo,
                Brand = "Mitsubishi",
                Model = "Xpander Cross",
                Seats = 7,
                ManufactureYear = 2021,
                CurrentKm = 52400,
                Color = "Bạc",
                ChassisNumber = "CAND-CH-003",
                EngineNumber = "CAND-EN-003",
                ReceiveDate = new DateTime(2021, 6, 10),
                OperationalStatus = VehicleOperationalStatus.InUse,
                RegistrationExpiryDate = homNay.AddMonths(5),
                NextMaintenanceDate = homNay.AddMonths(2),
                Note = "Xe đang thực hiện chuyến công tác phối hợp điều tra liên tỉnh."
            },

            // 4. Ô tô 5 chỗ Camry - Sẵn sàng
            new Vehicle
            {
                Id = 4,
                VehicleCode = "PT-004",
                LicensePlate = "80A-045.67",
                VehicleType = VehicleType.OTo,
                Brand = "Toyota",
                Model = "Camry 2.5Q",
                Seats = 5,
                ManufactureYear = 2022,
                CurrentKm = 31000,
                Color = "Đen",
                ChassisNumber = "CAND-CH-004",
                EngineNumber = "CAND-EN-004",
                ReceiveDate = new DateTime(2022, 5, 12),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(10),
                NextMaintenanceDate = homNay.AddMonths(5),
                Note = "Xe phục vụ công tác của Ban Chỉ huy đơn vị."
            },

            // 5. Ô tô 5 chỗ CX-5 - Đang bảo dưỡng (Maintenance)
            new Vehicle
            {
                Id = 5,
                VehicleCode = "PT-005",
                LicensePlate = "80A-056.78",
                VehicleType = VehicleType.OTo,
                Brand = "Mazda",
                Model = "CX-5 2.0",
                Seats = 5,
                ManufactureYear = 2020,
                CurrentKm = 64000,
                Color = "Xám",
                ChassisNumber = "CAND-CH-005",
                EngineNumber = "CAND-EN-005",
                ReceiveDate = new DateTime(2020, 8, 25),
                OperationalStatus = VehicleOperationalStatus.Maintenance,
                RegistrationExpiryDate = homNay.AddMonths(2),
                NextMaintenanceDate = homNay.AddDays(-2),
                Note = "Xe đang bảo dưỡng định kỳ cấp 60.000 km và thay thế hệ thống phanh tại xưởng."
            },

            // 6. Xe bán tải Ranger - Sẵn sàng
            new Vehicle
            {
                Id = 6,
                VehicleCode = "PT-006",
                LicensePlate = "80B-067.89",
                VehicleType = VehicleType.XeTai,
                Brand = "Ford",
                Model = "Ranger Wildtrak",
                Seats = 5,
                ManufactureYear = 2021,
                CurrentKm = 42100,
                Color = "Xanh rêu",
                ChassisNumber = "CAND-CH-006",
                EngineNumber = "CAND-EN-006",
                ReceiveDate = new DateTime(2021, 11, 5),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(7),
                NextMaintenanceDate = homNay.AddMonths(3),
                Note = "Xe chuyên dụng chở trang thiết bị kỹ thuật phục vụ công tác ngoại thành."
            },

            // 7. Xe bán tải D-Max - Ngừng hoạt động (Inactive)
            new Vehicle
            {
                Id = 7,
                VehicleCode = "PT-007",
                LicensePlate = "80B-078.90",
                VehicleType = VehicleType.XeTai,
                Brand = "Isuzu",
                Model = "D-Max Hi-Lander",
                Seats = 5,
                ManufactureYear = 2018,
                CurrentKm = 89500,
                Color = "Trắng",
                ChassisNumber = "CAND-CH-007",
                EngineNumber = "CAND-EN-007",
                ReceiveDate = new DateTime(2018, 4, 18),
                OperationalStatus = VehicleOperationalStatus.Inactive,
                RegistrationExpiryDate = homNay.AddMonths(-1),
                NextMaintenanceDate = null,
                Note = "Phương tiện đã hết hạn đăng kiểm, đã có tờ trình chờ điều chuyển/thanh lý."
            },

            // 8. Xe máy công vụ Winner X - Sẵn sàng
            new Vehicle
            {
                Id = 8,
                VehicleCode = "PT-008",
                LicensePlate = "80M1-089.12",
                VehicleType = VehicleType.XeMay,
                Brand = "Honda",
                Model = "Winner X 150",
                Seats = 2,
                ManufactureYear = 2023,
                CurrentKm = 11200,
                Color = "Đen nhám",
                ChassisNumber = "CAND-CH-008",
                EngineNumber = "CAND-EN-008",
                ReceiveDate = new DateTime(2023, 7, 10),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddYears(1),
                NextMaintenanceDate = homNay.AddMonths(4),
                Note = "Xe mô tô công vụ phục vụ tuần tra mật phục và xử lý nhanh tin báo địa bàn."
            }
        };
    }
}
