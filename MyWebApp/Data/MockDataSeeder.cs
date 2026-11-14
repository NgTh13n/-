using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Data;

public static class MockDataSeeder
{
    /// <summary>
    /// Danh sách 3 đơn vị / phòng đội trực thuộc công an quận / huyện mô phỏng
    /// </summary>
    public static List<Department> TaoDanhSachDonVi()
    {
        return new List<Department>
        {
            new Department
            {
                Id = 1,
                Name = "Đội Tham mưu - Tổng hợp",
                Description = "Đầu mối tiếp nhận, tham mưu chỉ huy và điều phối phương tiện công tác."
            },
            new Department
            {
                Id = 2,
                Name = "Đội Cảnh sát ĐTTP về TTXH",
                Description = "Đội Cảnh sát điều tra tội phạm về trật tự xã hội (Hình sự)."
            },
            new Department
            {
                Id = 3,
                Name = "Đội Cảnh sát QLHC về TTXH",
                Description = "Đội Cảnh sát quản lý hành chính về trật tự xã hội."
            }
        };
    }

    /// <summary>
    /// Danh sách 12 cán bộ theo đúng chuẩn Đặc tả Mục 11.2:
    /// - Trải đều 3 phòng đội với các cấp bậc CAND
    /// - Có cán bộ đủ điều kiện tự lái (CanSelfDrive = true) và không đủ điều kiện (CanSelfDrive = false)
    /// </summary>
    public static List<Officer> TaoDanhSachCanBo()
    {
        return new List<Officer>
        {
            // Đội 2: Cảnh sát Hình sự
            new Officer
            {
                Id = 1,
                Code = "CB-001",
                FullName = "Nguyễn Minh An",
                RankOrTitle = "Thiếu tá - Phó Đội trưởng",
                DepartmentId = 2,
                Department = "Đội Cảnh sát ĐTTP về TTXH",
                IsActive = true,
                CanSelfDrive = true, // Đủ điều kiện tự lái
                Phone = "0901.234.501",
                Notes = "Cán bộ trinh sát giàu kinh nghiệm, có GPLX B2 còn hạn."
            },
            new Officer
            {
                Id = 2,
                Code = "CB-002",
                FullName = "Hoàng Quốc Việt",
                RankOrTitle = "Thượng úy - Cán bộ",
                DepartmentId = 2,
                Department = "Đội Cảnh sát ĐTTP về TTXH",
                IsActive = true,
                CanSelfDrive = true,
                Phone = "0901.234.502",
                Notes = "Tổ trọng án, có GPLX B2."
            },
            new Officer
            {
                Id = 3,
                Code = "CB-003",
                FullName = "Nguyễn Văn Hưng",
                RankOrTitle = "Thiếu úy - Cán bộ",
                DepartmentId = 2,
                Department = "Đội Cảnh sát ĐTTP về TTXH",
                IsActive = true,
                CanSelfDrive = false, // Không đủ điều kiện tự lái
                Phone = "0901.234.503",
                Notes = "Cán bộ trẻ mới tiếp nhận công tác, chưa qua sát hạch lái xe công vụ."
            },
            new Officer
            {
                Id = 4,
                Code = "CB-004",
                FullName = "Võ Thành Trung",
                RankOrTitle = "Đại úy - Cán bộ",
                DepartmentId = 2,
                Department = "Đội Cảnh sát ĐTTP về TTXH",
                IsActive = true,
                CanSelfDrive = true,
                Phone = "0901.234.504",
                Notes = "Tổ phòng chống tội phạm cướp giật."
            },

            // Đội 1: Ban Chỉ huy & Tham mưu
            new Officer
            {
                Id = 5,
                Code = "CB-005",
                FullName = "Trần Quốc Bình",
                RankOrTitle = "Thượng tá - Phó Trưởng Công an",
                DepartmentId = 1,
                Department = "Đội Tham mưu - Tổng hợp",
                IsActive = true,
                CanSelfDrive = false, // Lãnh đạo đi công tác có lái xe riêng
                Phone = "0901.234.505",
                Notes = "Lãnh đạo phụ trách khối Tham mưu, Hậu cần và phê duyệt phương tiện."
            },
            new Officer
            {
                Id = 6,
                Code = "CB-006",
                FullName = "Lê Hoàng Nam",
                RankOrTitle = "Đại úy - Cán bộ Quản lý xe",
                DepartmentId = 1,
                Department = "Đội Tham mưu - Tổng hợp",
                IsActive = true,
                CanSelfDrive = true,
                Phone = "0901.234.506",
                Notes = "Cán bộ chuyên trách quản lý, điều phối đội phương tiện và xưởng kỹ thuật."
            },
            new Officer
            {
                Id = 7,
                Code = "CB-007",
                FullName = "Phạm Thu Hà",
                RankOrTitle = "Trung tá - Đội trưởng Tham mưu",
                DepartmentId = 1,
                Department = "Đội Tham mưu - Tổng hợp",
                IsActive = true,
                CanSelfDrive = true,
                Phone = "0901.234.507",
                Notes = "Quản trị hệ thống, phụ trách kế hoạch tổng hợp."
            },

            // Đội 1: Lực lượng lái xe chuyên trách
            new Officer
            {
                Id = 8,
                Code = "CB-008",
                FullName = "Vũ Đức Long",
                RankOrTitle = "Đại úy - Lái xe chuyên trách",
                DepartmentId = 1,
                Department = "Đội Tham mưu - Tổng hợp",
                IsActive = true,
                CanSelfDrive = true,
                Phone = "0910.000.001",
                Notes = "Lái xe cơ quan, GPLX B2, 12 năm kinh nghiệm."
            },
            new Officer
            {
                Id = 9,
                Code = "CB-009",
                FullName = "Đỗ Văn Hùng",
                RankOrTitle = "Thượng úy - Lái xe chuyên trách",
                DepartmentId = 1,
                Department = "Đội Tham mưu - Tổng hợp",
                IsActive = true,
                CanSelfDrive = true,
                Phone = "0910.000.002",
                Notes = "Lái xe cơ quan, GPLX C chuyên lái xe bán tải và xe thùng chuyên dụng."
            },
            new Officer
            {
                Id = 10,
                Code = "CB-010",
                FullName = "Ngô Quang Huy",
                RankOrTitle = "Thượng úy - Lái xe chuyên trách",
                DepartmentId = 1,
                Department = "Đội Tham mưu - Tổng hợp",
                IsActive = true,
                CanSelfDrive = true,
                Phone = "0910.000.003",
                Notes = "Lái xe cơ quan, GPLX D lái được cả xe khách và ô tô con."
            },
            new Officer
            {
                Id = 11,
                Code = "CB-011",
                FullName = "Bùi Thanh Tùng",
                RankOrTitle = "Trung úy - Lái xe chuyên trách",
                DepartmentId = 1,
                Department = "Đội Tham mưu - Tổng hợp",
                IsActive = true,
                CanSelfDrive = true,
                Phone = "0910.000.004",
                Notes = "Lái xe dự bị (GPLX mô phỏng đã hết hạn để kiểm tra điều phối)."
            },

            // Đội 3: Quản lý hành chính
            new Officer
            {
                Id = 12,
                Code = "CB-012",
                FullName = "Đặng Thùy Dung",
                RankOrTitle = "Đại úy - Phó Đội trưởng QLHC",
                DepartmentId = 3,
                Department = "Đội Cảnh sát QLHC về TTXH",
                IsActive = true,
                CanSelfDrive = false, // Chưa sát hạch lái xe công vụ
                Phone = "0901.234.512",
                Notes = "Phụ trách công tác cấp CCCD và quản lý ngành nghề kinh doanh có điều kiện."
            }
        };
    }

    /// <summary>
    /// Danh sách người dùng mô phỏng đăng nhập và vai trò demo
    /// </summary>
    public static List<User> TaoDanhSachNguoiDung()
    {
        return new List<User>
        {
            new User
            {
                Id = 1,
                Username = "canbo",
                Password = "123",
                FullName = "Nguyễn Minh An",
                DepartmentId = 2,
                Role = UserRole.Officer,
                Phone = "0901.234.501",
                OfficerId = 1
            },
            new User
            {
                Id = 2,
                Username = "lanhdao",
                Password = "123",
                FullName = "Trần Quốc Bình",
                DepartmentId = 1,
                Role = UserRole.Leader,
                Phone = "0901.234.505",
                OfficerId = 5
            },
            new User
            {
                Id = 3,
                Username = "doixe",
                Password = "123",
                FullName = "Lê Hoàng Nam",
                DepartmentId = 1,
                Role = UserRole.FleetManager,
                Phone = "0901.234.506",
                OfficerId = 6
            },
            new User
            {
                Id = 4,
                Username = "admin",
                Password = "123",
                FullName = "Phạm Thu Hà",
                DepartmentId = 1,
                Role = UserRole.Administrator,
                Phone = "0901.234.507",
                OfficerId = 7
            }
        };
    }

    /// <summary>
    /// Danh sách 4 Lái xe chuyên trách theo Mục 11.2:
    /// - Tham chiếu OfficerId
    /// - Có số GPLX và ngày hết hạn demo
    /// - 1 lái xe bằng lái hết hạn để test cảnh báo/chặn xếp xe
    /// </summary>
    public static List<Driver> TaoDanhSachLaiXe()
    {
        DateTime homNay = DateTime.Today;

        return new List<Driver>
        {
            new Driver
            {
                Id = 1,
                OfficerId = 8,
                FullName = "Vũ Đức Long",
                Phone = "0910.000.001",
                LicenseNumberDemo = "GPLX-29B2-08129",
                LicenseCategory = "B2",
                LicenseExpiryDate = homNay.AddYears(3),
                IsActive = true,
                Status = DriverStatus.Active,
                Notes = "Lái xe chính, nhiều năm kinh nghiệm lái xe chỉ huy."
            },
            new Driver
            {
                Id = 2,
                OfficerId = 9,
                FullName = "Đỗ Văn Hùng",
                Phone = "0910.000.002",
                LicenseNumberDemo = "GPLX-29C-04821",
                LicenseCategory = "C",
                LicenseExpiryDate = homNay.AddYears(2),
                IsActive = true,
                Status = DriverStatus.Active,
                Notes = "Lái xe tải và xe bán tải, sẵn sàng công tác xa."
            },
            new Driver
            {
                Id = 3,
                OfficerId = 10,
                FullName = "Ngô Quang Huy",
                Phone = "0910.000.003",
                LicenseNumberDemo = "GPLX-29D-09214",
                LicenseCategory = "D",
                LicenseExpiryDate = homNay.AddYears(4),
                IsActive = true,
                Status = DriverStatus.Active,
                Notes = "Lái xe khách và xe phục vụ đoàn."
            },
            new Driver
            {
                Id = 4,
                OfficerId = 11,
                FullName = "Bùi Thanh Tùng",
                Phone = "0910.000.004",
                LicenseNumberDemo = "GPLX-29C-01128",
                LicenseCategory = "C",
                // Giấy phép lái xe hết hạn 2 ngày trước để thử nghiệm kiểm tra tính hợp lệ khi điều phối!
                LicenseExpiryDate = homNay.AddDays(-2),
                IsActive = true,
                Status = DriverStatus.Active,
                Notes = "CẢNH BÁO: Giấy phép lái xe đã hết hạn ngày " + homNay.AddDays(-2).ToString("dd/MM/yyyy") + ", cần làm thủ tục đổi bằng."
            }
        };
    }

    /// <summary>
    /// Danh sách Yêu cầu sử dụng phương tiện đủ mọi trạng thái theo Mục 11.3:
    /// - 2 Chờ phê duyệt
    /// - 1 Yêu cầu bổ sung
    /// - 1 Bị từ chối
    /// - 1 Đã duyệt chờ điều phối
    /// - 1 Đã điều phối cho tương lai
    /// - 1 Đang sử dụng
    /// - 1 Hoàn thành
    /// - 1 Đã hủy
    /// </summary>
    public static List<Booking> TaoDanhSachYeuCau()
    {
        DateTime homNay = DateTime.Today;

        return new List<Booking>
        {
            // 1. Chờ phê duyệt - YC 1 (Cán bộ tự lái)
            new Booking
            {
                Id = 1,
                BookingCode = $"YC-{homNay.Year}-001",
                RequesterId = 1, // Nguyễn Minh An (CanSelfDrive = true)
                DepartmentId = 2,
                DepartmentSnapshot = "Đội Cảnh sát ĐTTP về TTXH",
                WorkType = "Tuần tra kiểm soát",
                Purpose = "Tuần tra kiểm soát ban đêm tại địa bàn trọng điểm phức tạp về ANTT",
                TaskDescription = "Phối hợp với lực lượng cơ sở tuần tra phòng chống tội phạm trộm cắp, cướp giật trên các tuyến phố chính.",
                Departure = "Trụ sở Công an",
                Destination = "Khu đô thị Kim Văn Kim Lũ và tuyến Vành đai 3",
                DepartureTime = homNay.AddDays(1).AddHours(20),
                ReturnTime = homNay.AddDays(1).AddHours(23).AddMinutes(30),
                PassengerCount = 3,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.SelfDrive, // Đề nghị Cán bộ tự lái
                Status = BookingStatus.PendingApproval,
                CreatedAt = homNay.AddHours(-4)
            },

            // 2. Chờ phê duyệt - YC 2 (Cần Lái xe đơn vị)
            new Booking
            {
                Id = 2,
                BookingCode = $"YC-{homNay.Year}-002",
                RequesterId = 2, // Hoàng Quốc Việt
                DepartmentId = 2,
                DepartmentSnapshot = "Đội Cảnh sát ĐTTP về TTXH",
                WorkType = "Làm việc theo lịch",
                Purpose = "Làm việc với Tòa án nhân dân và Viện Kiểm sát nhân dân TP Hà Nội",
                TaskDescription = "Chuyển giao hồ sơ vụ án ma túy và trích lục bản án phục vụ điều tra mở rộng.",
                Departure = "Trụ sở Công an",
                Destination = "TAND TP Hà Nội (đường Phạm Văn Bạch, Cầu Giấy)",
                DepartureTime = homNay.AddDays(2).AddHours(8),
                ReturnTime = homNay.AddDays(2).AddHours(11).AddMinutes(30),
                PassengerCount = 2,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.AssignedDriver, // Đề nghị phân công lái xe
                Status = BookingStatus.PendingApproval,
                CreatedAt = homNay.AddHours(-2)
            },

            // 3. Yêu cầu bổ sung - YC 3
            new Booking
            {
                Id = 3,
                BookingCode = $"YC-{homNay.Year}-003",
                RequesterId = 1, // Nguyễn Minh An
                DepartmentId = 2,
                DepartmentSnapshot = "Đội Cảnh sát ĐTTP về TTXH",
                WorkType = "Công tác địa bàn",
                Purpose = "Xác minh nhân thân lai lịch đối tượng trọng điểm",
                TaskDescription = "Thu thập thông tin di biến động của nhóm đối tượng có biểu hiện hoạt động tín dụng đen.",
                Departure = "Trụ sở Công an",
                Destination = "Khu công nghiệp Vĩnh Tuy và cảng Khuyến Lương",
                DepartureTime = homNay.AddDays(2).AddHours(13).AddMinutes(30),
                ReturnTime = homNay.AddDays(2).AddHours(17),
                PassengerCount = 4,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.AssignedDriver,
                Status = BookingStatus.NeedAdjustment,
                NeedAdjustmentReason = "Đề nghị bổ sung danh sách cán bộ cùng đi và xác nhận của Đội trưởng về kế hoạch công tác ngoại thành.",
                ApprovalOfficerId = 5, // Lãnh đạo Trần Quốc Bình
                ApprovalAt = homNay.AddHours(-1),
                CreatedAt = homNay.AddHours(-6)
            },

            // 4. Bị từ chối - YC 4
            new Booking
            {
                Id = 4,
                BookingCode = $"YC-{homNay.Year}-004",
                RequesterId = 3, // Nguyễn Văn Hưng
                DepartmentId = 2,
                DepartmentSnapshot = "Đội Cảnh sát ĐTTP về TTXH",
                WorkType = "Công tác đột xuất",
                Purpose = "Đi công tác ngoại tỉnh thu thập tài liệu chứng cứ",
                TaskDescription = "Đi các tỉnh lân cận thẩm tra xác minh nguồn gốc tang vật.",
                Departure = "Trụ sở Công an",
                Destination = "TP Phủ Lý, tỉnh Hà Nam",
                DepartureTime = homNay.AddDays(3).AddHours(6),
                ReturnTime = homNay.AddDays(3).AddHours(19),
                PassengerCount = 2,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.AssignedDriver,
                Status = BookingStatus.Rejected,
                RejectionReason = "Kế hoạch công tác ngoại tỉnh chưa có văn bản phê duyệt của Ban Giám đốc Công an tỉnh/thành phố.",
                ApprovalOfficerId = 5,
                ApprovalAt = homNay.AddHours(-12),
                CreatedAt = homNay.AddDays(-1)
            },

            // 5. Đã duyệt chờ điều phối - YC 5
            new Booking
            {
                Id = 5,
                BookingCode = $"YC-{homNay.Year}-005",
                RequesterId = 12, // Đặng Thùy Dung
                DepartmentId = 3,
                DepartmentSnapshot = "Đội Cảnh sát QLHC về TTXH",
                WorkType = "Kiểm tra cơ sở",
                Purpose = "Kiểm tra liên ngành an toàn PCCC và ANTT các cơ sở karaoke, lưu trú",
                TaskDescription = "Kiểm tra đột xuất 15 cơ sở kinh doanh có điều kiện về ANTT trên địa bàn quản lý.",
                Departure = "Trụ sở Công an",
                Destination = "Các cơ sở kinh doanh trên địa bàn quận/huyện",
                DepartureTime = homNay.AddDays(1).AddHours(14),
                ReturnTime = homNay.AddDays(1).AddHours(18),
                PassengerCount = 4,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.AssignedDriver,
                Status = BookingStatus.Approved,
                ApprovalOfficerId = 5,
                ApprovalAt = homNay.AddHours(-5),
                ApprovalComment = "Đồng ý phê duyệt, giao Quản lý đội xe bố trí phương tiện và lái xe đảm bảo an toàn.",
                CreatedAt = homNay.AddHours(-8)
            },

            // 6. Đã điều phối cho tương lai - YC 6 (Gắn với Trip 1)
            new Booking
            {
                Id = 6,
                BookingCode = $"YC-{homNay.Year}-006",
                RequesterId = 7, // Phạm Thu Hà
                DepartmentId = 1,
                DepartmentSnapshot = "Đội Tham mưu - Tổng hợp",
                WorkType = "Đón tiếp đoàn",
                Purpose = "Đón tiếp và phục vụ Đoàn kiểm tra của Bộ Công an về làm việc",
                TaskDescription = "Đưa đón Đoàn đại biểu từ sân bay Nội Bài về trụ sở làm việc và bố trí lưu trú.",
                Departure = "Trụ sở Công an",
                Destination = "Sân bay Quốc tế Nội Bài - Trụ sở Công an",
                DepartureTime = homNay.AddDays(1).AddHours(7).AddMinutes(30),
                ReturnTime = homNay.AddDays(1).AddHours(17).AddMinutes(30),
                PassengerCount = 5,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.AssignedDriver,
                Status = BookingStatus.Dispatched,
                ApprovalOfficerId = 5,
                ApprovalAt = homNay.AddDays(-1).AddHours(15),
                ApprovalComment = "Phê duyệt đón tiếp đoàn công tác Bộ Công an chu đáo, trang trọng.",
                CreatedAt = homNay.AddDays(-1).AddHours(10)
            },

            // 7. Đang sử dụng - YC 7 (Gắn với Trip 2, Cán bộ tự lái)
            new Booking
            {
                Id = 7,
                BookingCode = $"YC-{homNay.Year}-007",
                RequesterId = 1, // Nguyễn Minh An (CanSelfDrive = true)
                DepartmentId = 2,
                DepartmentSnapshot = "Đội Cảnh sát ĐTTP về TTXH",
                WorkType = "Phối hợp phá án",
                Purpose = "Phối hợp mật phục, bắt giữ đối tượng truy nã đặc biệt nguy hiểm",
                TaskDescription = "Tổ trinh sát phối hợp với Phòng Cảnh sát hình sự triển khai phương án vây bắt đối tượng theo Chuyên án 112-T.",
                Departure = "Trụ sở Công an",
                Destination = "Thị xã Phú Thọ và huyện Tam Nông, tỉnh Phú Thọ",
                DepartureTime = homNay.AddHours(6),
                ReturnTime = homNay.AddHours(20),
                PassengerCount = 4,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.SelfDrive, // Cán bộ tự lái
                Status = BookingStatus.InUse,
                ApprovalOfficerId = 5,
                ApprovalAt = homNay.AddDays(-1).AddHours(18),
                ApprovalComment = "Duyệt lệnh công tác khẩn cấp. Yêu cầu tổ công tác tuyệt đối giữ bí mật và an toàn.",
                CreatedAt = homNay.AddDays(-1).AddHours(16)
            },

            // 8. Đã hoàn thành - YC 8 (Gắn với Trip 3)
            new Booking
            {
                Id = 8,
                BookingCode = $"YC-{homNay.Year}-008",
                RequesterId = 2, // Hoàng Quốc Việt
                DepartmentId = 2,
                DepartmentSnapshot = "Đội Cảnh sát ĐTTP về TTXH",
                WorkType = "Áp giải đối tượng",
                Purpose = "Áp giải dẫn giải đối tượng vi phạm pháp luật về Trại tạm giam",
                TaskDescription = "Thực hiện lệnh tạm giam, bàn giao đối tượng và hồ sơ tạm giam theo quy định.",
                Departure = "Trụ sở Công an",
                Destination = "Trại tạm giam số 1 - Công an TP Hà Nội (Xuân Phương, Nam Từ Liêm)",
                DepartureTime = homNay.AddDays(-1).AddHours(8),
                ReturnTime = homNay.AddDays(-1).AddHours(16).AddMinutes(30),
                PassengerCount = 4,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.AssignedDriver,
                Status = BookingStatus.Completed,
                ApprovalOfficerId = 5,
                ApprovalAt = homNay.AddDays(-2).AddHours(16),
                CreatedAt = homNay.AddDays(-2).AddHours(14)
            },

            // 9. Đã hủy - YC 9
            new Booking
            {
                Id = 9,
                BookingCode = $"YC-{homNay.Year}-009",
                RequesterId = 4, // Võ Thành Trung
                DepartmentId = 2,
                DepartmentSnapshot = "Đội Cảnh sát ĐTTP về TTXH",
                WorkType = "Công tác địa bàn",
                Purpose = "Xác minh thông tin tố giác tội phạm tại phường Mai Dịch",
                TaskDescription = "Kiểm tra hiện trường nơi xảy ra tranh chấp có dấu hiệu hủy hoại tài sản.",
                Departure = "Trụ sở Công an",
                Destination = "Phường Mai Dịch, Cầu Giấy, Hà Nội",
                DepartureTime = homNay.AddDays(-1).AddHours(14),
                ReturnTime = homNay.AddDays(-1).AddHours(17),
                PassengerCount = 2,
                RequestedVehicleType = "Ô tô",
                PreferredDriverMode = DriverMode.SelfDrive,
                Status = BookingStatus.Cancelled,
                CancellationReason = "Đối tượng tố giác rút đơn, hai bên tự hòa giải thành công nên hủy lịch xe.",
                CreatedAt = homNay.AddDays(-2).AddHours(9)
            }
        };
    }

    /// <summary>
    /// Danh sách Chuyến công tác (Trips) theo Mục 11.3:
    /// - 1 chuyến đã điều phối cho tương lai
    /// - 1 chuyến đang sử dụng (có biên bản bàn giao)
    /// - 1 chuyến hoàn thành (có biên bản bàn giao, hoàn trả, số km, nhiên liệu)
    /// </summary>
    public static List<Trip> TaoDanhSachChuyenDi()
    {
        DateTime homNay = DateTime.Today;

        return new List<Trip>
        {
            // 1. Chuyến đã điều phối cho tương lai (YC 6) - Lái xe đơn vị
            new Trip
            {
                Id = 1,
                DispatchCode = $"LĐĐ-{homNay.Year}-001",
                BookingId = 6,
                VehicleId = 2, // Hyundai Santa Fe 80A-023.45
                DriverMode = DriverMode.AssignedDriver,
                DriverId = 1,  // Vũ Đức Long
                SelfDrivingOfficerId = null,
                DriverNameSnapshot = "Vũ Đức Long",
                RequesterNameSnapshot = "Phạm Thu Hà",
                VehiclePlateSnapshot = "80A-023.45",
                ScheduledStartAt = homNay.AddDays(1).AddHours(7).AddMinutes(30),
                ScheduledEndAt = homNay.AddDays(1).AddHours(17).AddMinutes(30),
                DispatcherId = 6, // Lê Hoàng Nam
                DispatchedAt = homNay.AddDays(-1).AddHours(16).AddMinutes(30),
                Status = TripStatus.WaitingDeparture,
                StartKm = 18200,
                EndKm = 0,
                FuelUsed = 0,
                IsSettled = false,
                Note = "Lệnh điều động đón đoàn kiểm tra Bộ Công an. Yêu cầu kiểm tra xe chu đáo trước giờ đón."
            },

            // 2. Chuyến đang sử dụng (YC 7) - Cán bộ tự lái (SelfDrive)
            new Trip
            {
                Id = 2,
                DispatchCode = $"LĐĐ-{homNay.Year}-002",
                BookingId = 7,
                VehicleId = 3, // Mitsubishi Xpander 80A-034.56
                DriverMode = DriverMode.SelfDrive,
                DriverId = 0,
                SelfDrivingOfficerId = 1, // Nguyễn Minh An (CanSelfDrive = true)
                DriverNameSnapshot = "Nguyễn Minh An (Cán bộ tự lái)",
                RequesterNameSnapshot = "Nguyễn Minh An",
                VehiclePlateSnapshot = "80A-034.56",
                ScheduledStartAt = homNay.AddHours(6),
                ScheduledEndAt = homNay.AddHours(20),
                DispatcherId = 6,
                DispatchedAt = homNay.AddDays(-1).AddHours(19),
                Status = TripStatus.InTransit, // Đang di chuyển / InUse
                HandoverAt = homNay.AddHours(6).AddMinutes(15),
                StartKm = 52400,
                FuelStartLevel = "100% (Đầy bình)",
                ConditionBefore = "Phương tiện hoạt động tốt, đã bàn giao đủ giấy tờ đăng ký, đăng kiểm, lốp dự phòng, kích xe.",
                EndKm = 0,
                FuelUsed = 0,
                IsSettled = false,
                Note = "Chuyến đi chuyên án Phú Thọ. Đồng chí Nguyễn Minh An trực tiếp điều khiển phương tiện."
            },

            // 3. Chuyến đã hoàn thành và quyết toán (YC 8) - Lái xe đơn vị
            new Trip
            {
                Id = 3,
                DispatchCode = $"LĐĐ-{homNay.Year}-003",
                BookingId = 8,
                VehicleId = 1, // Toyota Fortuner 80A-012.34
                DriverMode = DriverMode.AssignedDriver,
                DriverId = 2,  // Đỗ Văn Hùng
                SelfDrivingOfficerId = null,
                DriverNameSnapshot = "Đỗ Văn Hùng",
                RequesterNameSnapshot = "Hoàng Quốc Việt",
                VehiclePlateSnapshot = "80A-012.34",
                ScheduledStartAt = homNay.AddDays(-1).AddHours(8),
                ScheduledEndAt = homNay.AddDays(-1).AddHours(16).AddMinutes(30),
                DispatcherId = 6,
                DispatchedAt = homNay.AddDays(-2).AddHours(17),
                Status = TripStatus.Returned, // Đã về và hoàn tất quyết toán
                HandoverAt = homNay.AddDays(-1).AddHours(7).AddMinutes(50),
                StartKm = 28400,
                FuelStartLevel = "100%",
                ConditionBefore = "Xe sạch sẽ, hoạt động ổn định, đèn còi còi ưu tiên hoạt động tốt.",
                ReturnedAt = homNay.AddDays(-1).AddHours(16).AddMinutes(45),
                EndKm = 28500,
                FuelEndLevel = "80%",
                ConditionAfter = "Xe nguyên vẹn, nội thất sạch sẽ, không có trầy xước phát sinh.",
                HasIncident = false,
                IncidentNote = "",
                FuelUsed = 12.5, // 12.5 lít dầu
                IsSettled = true,
                Note = "Đã hoàn tất quyết toán chuyến đi áp giải can phạm. Xe bàn giao lại cho Đội xe trực chiến."
            }
        };
    }

    /// <summary>
    /// Danh sách Hồ sơ bảo dưỡng kỹ thuật phương tiện theo Mục 11.3:
    /// - 1 bản ghi đang thực hiện / sắp đến hạn (PT-005 Mazda CX-5)
    /// - 1 bản ghi đã hoàn tất (PT-001 Toyota Fortuner)
    /// </summary>
    public static List<MaintenanceRecord> TaoDanhSachBaoDuong()
    {
        DateTime homNay = DateTime.Today;

        return new List<MaintenanceRecord>
        {
            // Bản ghi 1: Đang thực hiện bảo dưỡng (Xe PT-005 CX-5)
            new MaintenanceRecord
            {
                Id = 1,
                VehicleId = 5, // Mazda CX-5 80A-056.78
                MaintenanceType = MaintenanceType.Periodic,
                Description = "Bảo dưỡng định kỳ cấp 60.000 km: Thay dầu động cơ, lọc dầu, lọc gió máy lạnh, bảo dưỡng toàn bộ 4 cụm phanh, kiểm tra góc đặt bánh xe.",
                PlannedAt = homNay.AddDays(-2),
                CompletedAt = null,
                Status = MaintenanceStatus.InProgress,
                OdometerKm = 64000,
                GarageName = "Xưởng Sửa chữa Kỹ thuật Ô tô CAND - Cục Hậu cần",
                Cost = 3850000,
                Notes = "Xe đang nằm xưởng hoàn thiện thay má phanh trước. Dự kiến ngày mai nghiệm thu xuất xưởng."
            },

            // Bản ghi 2: Đã hoàn tất bảo dưỡng và kiểm định (Xe PT-001 Fortuner)
            new MaintenanceRecord
            {
                Id = 2,
                VehicleId = 1, // Toyota Fortuner 80A-012.34
                MaintenanceType = MaintenanceType.Inspection,
                Description = "Đăng kiểm định kỳ và kiểm định khí thải, an toàn kỹ thuật phương tiện cơ giới đường bộ.",
                PlannedAt = homNay.AddMonths(-2),
                CompletedAt = homNay.AddMonths(-2).AddDays(1),
                Status = MaintenanceStatus.Completed,
                OdometerKm = 25000,
                GarageName = "Trung tâm Đăng kiểm xe cơ giới 29-03V Hà Nội",
                Cost = 1450000,
                Notes = "Đã dán tem đăng kiểm mới có hiệu lực đến " + homNay.AddMonths(8).ToString("dd/MM/yyyy") + ". Phương tiện đạt tiêu chuẩn vận hành an toàn."
            }
        };
    }

    /// <summary>
    /// Danh sách Lịch sử trạng thái (StatusHistory / Audit Trail) mẫu
    /// </summary>
    public static List<StatusHistory> TaoDanhSachLichSuTrangThai()
    {
        DateTime homNay = DateTime.Today;

        return new List<StatusHistory>
        {
            new StatusHistory
            {
                Id = 1,
                EntityType = "VehicleRequest",
                EntityId = 5,
                FromStatus = "PendingApproval",
                ToStatus = "Approved",
                ChangedAt = homNay.AddHours(-5),
                ChangedByOfficerId = 5,
                ChangedByFullName = "Trần Quốc Bình",
                Comment = "Đồng ý phê duyệt, giao Quản lý đội xe bố trí phương tiện và lái xe đảm bảo an toàn."
            },
            new StatusHistory
            {
                Id = 2,
                EntityType = "VehicleRequest",
                EntityId = 3,
                FromStatus = "PendingApproval",
                ToStatus = "NeedAdjustment",
                ChangedAt = homNay.AddHours(-1),
                ChangedByOfficerId = 5,
                ChangedByFullName = "Trần Quốc Bình",
                Comment = "Đề nghị bổ sung danh sách cán bộ cùng đi và xác nhận của Đội trưởng về kế hoạch công tác ngoại thành."
            },
            new StatusHistory
            {
                Id = 3,
                EntityType = "VehicleRequest",
                EntityId = 4,
                FromStatus = "PendingApproval",
                ToStatus = "Rejected",
                ChangedAt = homNay.AddHours(-12),
                ChangedByOfficerId = 5,
                ChangedByFullName = "Trần Quốc Bình",
                Comment = "Kế hoạch công tác ngoại tỉnh chưa có văn bản phê duyệt của Ban Giám đốc Công an tỉnh/thành phố."
            },
            new StatusHistory
            {
                Id = 4,
                EntityType = "Trip",
                EntityId = 2,
                FromStatus = "WaitingDeparture",
                ToStatus = "InTransit",
                ChangedAt = homNay.AddHours(6).AddMinutes(15),
                ChangedByOfficerId = 6,
                ChangedByFullName = "Lê Hoàng Nam",
                Comment = "Bàn giao phương tiện 80A-034.56 cho cán bộ Nguyễn Minh An tự lái. Km xuất phát: 52.400 km."
            },
            new StatusHistory
            {
                Id = 5,
                EntityType = "Trip",
                EntityId = 3,
                FromStatus = "InTransit",
                ToStatus = "Returned",
                ChangedAt = homNay.AddDays(-1).AddHours(16).AddMinutes(45),
                ChangedByOfficerId = 6,
                ChangedByFullName = "Lê Hoàng Nam",
                Comment = "Nghiệm thu hoàn trả phương tiện 80A-012.34. Km về: 28.500 km. Chuyến đi an toàn, đã quyết toán."
            }
        };
    }

    /// <summary>
    /// Nhật ký hoạt động chung của hệ thống
    /// </summary>
    public static List<ActivityLog> TaoDanhSachNhatKy()
    {
        DateTime homNay = DateTime.Today;

        return new List<ActivityLog>
        {
            new ActivityLog
            {
                Id = 1,
                UserId = 1,
                Action = "Đăng ký phương tiện",
                Description = "Cán bộ Nguyễn Minh An gửi yêu cầu sử dụng xe đi tuần tra kiểm soát ban đêm.",
                CreatedAt = homNay.AddHours(-4)
            },
            new ActivityLog
            {
                Id = 2,
                UserId = 2,
                Action = "Phê duyệt yêu cầu",
                Description = "Lãnh đạo Trần Quốc Bình phê duyệt yêu cầu YC-2026-005 của Đội QLHC.",
                CreatedAt = homNay.AddHours(-5)
            },
            new ActivityLog
            {
                Id = 3,
                UserId = 3,
                Action = "Bàn giao xe",
                Description = "Cán bộ Quản lý xe Lê Hoàng Nam đã bàn giao xe 80A-034.56 cho Thiếu tá Nguyễn Minh An tự lái.",
                CreatedAt = homNay.AddHours(6).AddMinutes(15)
            },
            new ActivityLog
            {
                Id = 4,
                UserId = 3,
                Action = "Quyết toán chuyến đi",
                Description = "Hoàn tất quyết toán chuyến LĐĐ-2026-003 cho xe 80A-012.34 (lái xe Đỗ Văn Hùng).",
                CreatedAt = homNay.AddDays(-1).AddHours(17)
            }
        };
    }
}
