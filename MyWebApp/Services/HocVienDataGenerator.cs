
using MyWebApp.Services.Models.CanBo;
using System;
using System.Collections.Generic;
using System.Linq;

public static class HocVienDataGenerator
{
    // Danh sách được khởi tạo một lần và lưu trong RAM.
    public static List<tbHocVien> DanhSach { get; set; } = TaoDanhSach();

    private static List<tbHocVien> TaoDanhSach()
    {
        string[] ho =
        {
            "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng",
            "Huỳnh", "Phan", "Vũ", "Đặng", "Bùi"
        };

        string[] demNam =
        {
            "Văn", "Hữu", "Đức", "Quốc", "Minh"
        };

        string[] demNu =
        {
            "Thị", "Thu", "Ngọc", "Thanh", "Mai"
        };

        string[] tenNam =
        {
            "Anh", "Bình", "Cường", "Dũng", "Hải",
            "Hùng", "Long", "Nam", "Quân", "Tuấn"
        };

        string[] tenNu =
        {
            "An", "Chi", "Dung", "Giang", "Hà",
            "Hạnh", "Linh", "Mai", "Nga", "Trang"
        };

        string[] noiSinh =
        {
            "Hà Nội", "Hải Phòng", "Nam Định",
            "Thanh Hóa", "Nghệ An", "Đà Nẵng",
            "Thái Bình", "Bắc Ninh", "Hưng Yên",
            "Quảng Ninh"
        };

        string[] donVi =
        {
            "Khoa Nghiệp vụ 1",
            "Khoa Nghiệp vụ 2",
            "Khoa Quản lý nhà nước",
            "Khoa Công nghệ thông tin",
            "Khoa Luật",
            "Khoa Cảnh sát",
            "Khoa An ninh",
            "Khoa Ngoại ngữ",
            "Khoa Lý luận chính trị",
            "Khoa Khoa học cơ bản"
        };

        string[] lop =
        {
            "HV01", "HV02", "HV03", "HV04", "HV05"
        };

        string[] tenKhac =
        {
            "Minh Anh", "Hoàng Nam", "Thanh Bình",
            "Hải Đăng", "Bảo Ngọc", "Gia Hân",
            "Quốc Huy", "Khánh Linh", "Đức Anh",
            "Phương Thảo"
        };

        string[] duong =
        {
            "Nguyễn Trãi", "Lê Lợi", "Trần Phú",
            "Hoàng Diệu", "Lý Thường Kiệt",
            "Quang Trung", "Hai Bà Trưng",
            "Điện Biên Phủ", "Phan Chu Trinh",
            "Trường Chinh"
        };

        var danhSach = new List<tbHocVien>(100);

        for (int i = 0; i < 100; i++)
        {
            int stt = i + 1;
            bool laNam = i % 2 == 0;

            // Sinh họ tên theo tổ hợp để tạo dữ liệu đa dạng.
            string hoTen = string.Join(" ",
                ho[i % ho.Length],
                laNam
                    ? demNam[(i / 2) % demNam.Length]
                    : demNu[(i / 2) % demNu.Length],
                laNam
                    ? tenNam[(i / 2) % tenNam.Length]
                    : tenNu[(i / 2) % tenNu.Length]);

            // Ngày sinh mẫu trong khoảng 1990–2004.
            var ngaySinh = new DateTime(
                1990 + (i % 15),
                1 + (i % 12),
                1 + (i % 27));

            // Ngày vào Đảng mẫu, sau 18 tuổi.
            var ngayVaoDang = ngaySinh.AddYears(
                20 + (i % 5));
                                                                                                       
            var hocVien = new tbHocVien
            {
                IdCanBo = stt,
                SoHieuCAND = $"{500 + stt:D3}",
                GioiTinh = laNam ? "Nam" : "Nữ",
                HoTen = hoTen,
                TenGoiKhac = tenKhac[i % tenKhac.Length],
                NgaySinh = ngaySinh,
                NoiSinh = $"Phường trung tâm, {noiSinh[i % noiSinh.Length]}",
                NgayVaoDang = ngayVaoDang,
                NgayChinhThuc = ngayVaoDang.AddYears(1),
                DonViCT = donVi[i % donVi.Length],
                Lop = lop[i % lop.Length],
                DiaChiHienTai =
                    $"{stt} {duong[i % duong.Length]}, {noiSinh[i % noiSinh.Length]}"
            };

            danhSach.Add(hocVien);
        }

        return danhSach;
    }
}
