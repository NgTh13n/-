using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Data;

public static class VehicleData
{
    // Danh sách 10 phương tiện mẫu, được tạo lại khi ứng dụng khởi động.
    public static List<Vehicle> TaoDanhSach()
    {
        List<Vehicle> danhSach = new List<Vehicle>();

        danhSach.Add(new Vehicle
        {
            Id = 1,
            VehicleCode = "PT-001",
            LicensePlate = "29A-123.45",
            VehicleType = VehicleType.OTo,
            Brand = "Toyota",
            Model = "Fortuner",
            Seats = 7,
            ManufactureYear = 2022,
            CurrentKm = 28580,
            Color = "Trắng",
            ChassisNumber = "SAMPLE-CH-001",
            EngineNumber = "SAMPLE-EN-001",
            ReceiveDate = new DateTime(2022, 1, 15),
            Status = VehicleStatus.SanSang,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 2,
            VehicleCode = "PT-002",
            LicensePlate = "29A-234.56",
            VehicleType = VehicleType.OTo,
            Brand = "Ford",
            Model = "Ranger",
            Seats = 5,
            ManufactureYear = 2021,
            CurrentKm = 41300,
            Color = "Trắng",
            ChassisNumber = "SAMPLE-CH-002",
            EngineNumber = "SAMPLE-EN-002",
            ReceiveDate = new DateTime(2021, 1, 15),
            Status = VehicleStatus.SanSang,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 3,
            VehicleCode = "PT-003",
            LicensePlate = "29B-345.67",
            VehicleType = VehicleType.OTo,
            Brand = "Mitsubishi",
            Model = "Xpander",
            Seats = 7,
            ManufactureYear = 2020,
            CurrentKm = 56200,
            Color = "Trắng",
            ChassisNumber = "SAMPLE-CH-003",
            EngineNumber = "SAMPLE-EN-003",
            ReceiveDate = new DateTime(2020, 1, 15),
            Status = VehicleStatus.DangBaoDuong,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 4,
            VehicleCode = "PT-004",
            LicensePlate = "29B-456.78",
            VehicleType = VehicleType.OTo,
            Brand = "Hyundai",
            Model = "Santa Fe",
            Seats = 7,
            ManufactureYear = 2023,
            CurrentKm = 17800,
            Color = "Trắng",
            ChassisNumber = "SAMPLE-CH-004",
            EngineNumber = "SAMPLE-EN-004",
            ReceiveDate = new DateTime(2023, 1, 15),
            Status = VehicleStatus.DangDuocDieuDong,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 5,
            VehicleCode = "PT-005",
            LicensePlate = "29M1-123.45",
            VehicleType = VehicleType.XeMay,
            Brand = "Honda",
            Model = "Winner",
            Seats = 2,
            ManufactureYear = 2022,
            CurrentKm = 12600,
            Color = "Đỏ",
            ChassisNumber = "SAMPLE-CH-005",
            EngineNumber = "SAMPLE-EN-005",
            ReceiveDate = new DateTime(2022, 1, 15),
            Status = VehicleStatus.SanSang,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 6,
            VehicleCode = "PT-006",
            LicensePlate = "29M1-234.56",
            VehicleType = VehicleType.XeMay,
            Brand = "Yamaha",
            Model = "Exciter",
            Seats = 2,
            ManufactureYear = 2021,
            CurrentKm = 22100,
            Color = "Xanh",
            ChassisNumber = "SAMPLE-CH-006",
            EngineNumber = "SAMPLE-EN-006",
            ReceiveDate = new DateTime(2021, 1, 15),
            Status = VehicleStatus.NgungHoatDong,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 7,
            VehicleCode = "PT-007",
            LicensePlate = "29C-567.89",
            VehicleType = VehicleType.XeTai,
            Brand = "Isuzu",
            Model = "QKR",
            Seats = 3,
            ManufactureYear = 2019,
            CurrentKm = 68400,
            Color = "Trắng",
            ChassisNumber = "SAMPLE-CH-007",
            EngineNumber = "SAMPLE-EN-007",
            ReceiveDate = new DateTime(2019, 1, 15),
            Status = VehicleStatus.SanSang,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 8,
            VehicleCode = "PT-008",
            LicensePlate = "29C-678.90",
            VehicleType = VehicleType.XeTai,
            Brand = "Hino",
            Model = "XZU",
            Seats = 3,
            ManufactureYear = 2020,
            CurrentKm = 49700,
            Color = "Trắng",
            ChassisNumber = "SAMPLE-CH-008",
            EngineNumber = "SAMPLE-EN-008",
            ReceiveDate = new DateTime(2020, 1, 15),
            Status = VehicleStatus.DangBaoDuong,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 9,
            VehicleCode = "PT-009",
            LicensePlate = "29A-789.01",
            VehicleType = VehicleType.XeChuyenDung,
            Brand = "Ford",
            Model = "Transit",
            Seats = 9,
            ManufactureYear = 2023,
            CurrentKm = 14200,
            Color = "Trắng",
            ChassisNumber = "SAMPLE-CH-009",
            EngineNumber = "SAMPLE-EN-009",
            ReceiveDate = new DateTime(2023, 1, 15),
            Status = VehicleStatus.SanSang,
            Note = "Dữ liệu mô phỏng"
        });

        danhSach.Add(new Vehicle
        {
            Id = 10,
            VehicleCode = "PT-010",
            LicensePlate = "29A-890.12",
            VehicleType = VehicleType.XeChuyenDung,
            Brand = "Toyota",
            Model = "Hiace",
            Seats = 9,
            ManufactureYear = 2022,
            CurrentKm = 30100,
            Color = "Trắng",
            ChassisNumber = "SAMPLE-CH-010",
            EngineNumber = "SAMPLE-EN-010",
            ReceiveDate = new DateTime(2022, 1, 15),
            Status = VehicleStatus.SanSang,
            Note = "Dữ liệu mô phỏng"
        });

        return danhSach;
    }
}
