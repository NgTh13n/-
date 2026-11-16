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
            },

            // 9. Ô tô 5 chỗ Camry Hybrid - Sẵn sàng (Phục vụ Ban Giám đốc / Đối ngoại)
            new Vehicle
            {
                Id = 9,
                VehicleCode = "PT-009",
                LicensePlate = "80A-068.86",
                VehicleType = VehicleType.OTo,
                Brand = "Toyota",
                Model = "Camry 2.5HEV Hybrid",
                Seats = 5,
                ManufactureYear = 2023,
                CurrentKm = 14500,
                Color = "Đen bóng",
                ChassisNumber = "CAND-CH-009",
                EngineNumber = "CAND-EN-009",
                ReceiveDate = new DateTime(2023, 4, 15),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(14),
                NextMaintenanceDate = homNay.AddMonths(5),
                Note = "Xe chuyên dụng phục vụ công tác Ban Chỉ huy và đối ngoại cấp cao."
            },

            // 10. Ô tô 7 chỗ Ford Everest 4x4 - Sẵn sàng (Tuần tra, công tác liên tỉnh địa hình khó)
            new Vehicle
            {
                Id = 10,
                VehicleCode = "PT-010",
                LicensePlate = "80A-079.99",
                VehicleType = VehicleType.OTo,
                Brand = "Ford",
                Model = "Everest Titanium 4x4",
                Seats = 7,
                ManufactureYear = 2023,
                CurrentKm = 21000,
                Color = "Trắng tuyết",
                ChassisNumber = "CAND-CH-010",
                EngineNumber = "CAND-EN-010",
                ReceiveDate = new DateTime(2023, 8, 12),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(11),
                NextMaintenanceDate = homNay.AddMonths(4),
                Note = "Xe 2 cầu chuyên dụng công tác vùng sâu vùng xa, phòng chống bão lụt thiên tai."
            },

            // 11. Bán tải đặc chủng Ranger Raptor - Sẵn sàng
            new Vehicle
            {
                Id = 11,
                VehicleCode = "PT-011",
                LicensePlate = "80B-082.35",
                VehicleType = VehicleType.XeTai,
                Brand = "Ford",
                Model = "Ranger Raptor Bi-Turbo",
                Seats = 5,
                ManufactureYear = 2022,
                CurrentKm = 36500,
                Color = "Xám xi măng",
                ChassisNumber = "CAND-CH-011",
                EngineNumber = "CAND-EN-011",
                ReceiveDate = new DateTime(2022, 10, 5),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(6),
                NextMaintenanceDate = homNay.AddMonths(2),
                Note = "Xe đặc chủng cơ động trinh sát phòng chống tội phạm ma túy và truy bắt đối tượng."
            },

            // 12. Ô tô khách 16 chỗ Ford Transit - Sẵn sàng (Chở quân, áp giải tập trung)
            new Vehicle
            {
                Id = 12,
                VehicleCode = "PT-012",
                LicensePlate = "80B-091.13",
                VehicleType = VehicleType.OTo,
                Brand = "Ford",
                Model = "Transit High Roof 16 Chỗ",
                Seats = 16,
                ManufactureYear = 2021,
                CurrentKm = 58200,
                Color = "Bạc ánh kim",
                ChassisNumber = "CAND-CH-012",
                EngineNumber = "CAND-EN-012",
                ReceiveDate = new DateTime(2021, 5, 20),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(4),
                NextMaintenanceDate = homNay.AddMonths(1),
                Note = "Xe chuyên dụng chở lực lượng làm nhiệm vụ cưỡng chế, bảo vệ sự kiện lớn và dẫn giải can phạm."
            },

            // 13. Ô tô điện VinFast VF8 Plus - Sẵn sàng
            new Vehicle
            {
                Id = 13,
                VehicleCode = "PT-013",
                LicensePlate = "80A-048.26",
                VehicleType = VehicleType.OTo,
                Brand = "VinFast",
                Model = "VF8 Plus Electric",
                Seats = 5,
                ManufactureYear = 2023,
                CurrentKm = 16800,
                Color = "Xanh CAND",
                ChassisNumber = "CAND-CH-013",
                EngineNumber = "CAND-EN-013",
                ReceiveDate = new DateTime(2023, 9, 1),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(15),
                NextMaintenanceDate = homNay.AddMonths(6),
                Note = "Phương tiện thuần điện công nghệ mới phục vụ tuần tra đô thị và xử lý văn bản khẩn."
            },

            // 14. Ô tô 7 chỗ Mitsubishi Pajero Sport - Đang công tác (InUse)
            new Vehicle
            {
                Id = 14,
                VehicleCode = "PT-014",
                LicensePlate = "80A-055.88",
                VehicleType = VehicleType.OTo,
                Brand = "Mitsubishi",
                Model = "Pajero Sport 4x4",
                Seats = 7,
                ManufactureYear = 2020,
                CurrentKm = 72100,
                Color = "Đen",
                ChassisNumber = "CAND-CH-014",
                EngineNumber = "CAND-EN-014",
                ReceiveDate = new DateTime(2020, 11, 14),
                OperationalStatus = VehicleOperationalStatus.InUse,
                RegistrationExpiryDate = homNay.AddMonths(5),
                NextMaintenanceDate = homNay.AddMonths(2),
                Note = "Đang thực hiện nhiệm vụ bảo đảm an ninh trật tự theo kế hoạch của Công an thành phố."
            },

            // 15. Xe tải chuyên dụng Isuzu QKR - Sẵn sàng
            new Vehicle
            {
                Id = 15,
                VehicleCode = "PT-015",
                LicensePlate = "80B-036.72",
                VehicleType = VehicleType.XeTai,
                Brand = "Isuzu",
                Model = "QKR 270 Thùng kín CAND",
                Seats = 3,
                ManufactureYear = 2021,
                CurrentKm = 46300,
                Color = "Trắng",
                ChassisNumber = "CAND-CH-015",
                EngineNumber = "CAND-EN-015",
                ReceiveDate = new DateTime(2021, 3, 10),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddMonths(8),
                NextMaintenanceDate = homNay.AddMonths(3),
                Note = "Xe tải chuyên dùng vận chuyển vũ khí quân trang, phương tiện kỹ thuật và vật chứng vụ án."
            },

            // 16. Xe mô tô công vụ tuần tra phản ứng nhanh - Sẵn sàng
            new Vehicle
            {
                Id = 16,
                VehicleCode = "PT-016",
                LicensePlate = "80M1-098.76",
                VehicleType = VehicleType.XeMay,
                Brand = "Yamaha",
                Model = "Exciter 155 VVA Cảnh sát",
                Seats = 2,
                ManufactureYear = 2024,
                CurrentKm = 4800,
                Color = "Trắng - Xanh",
                ChassisNumber = "CAND-CH-016",
                EngineNumber = "CAND-EN-016",
                ReceiveDate = new DateTime(2024, 2, 1),
                OperationalStatus = VehicleOperationalStatus.Ready,
                RegistrationExpiryDate = homNay.AddYears(2),
                NextMaintenanceDate = homNay.AddMonths(8),
                Note = "Trang bị còi ưu tiên, đèn chớp chuyên dụng phục vụ Đội Cảnh sát giao thông trật tự phản ứng nhanh."
            }
        };
    }
}
