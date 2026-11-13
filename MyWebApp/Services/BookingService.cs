using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Services;

public class BookingService
{
    private readonly MockDataService duLieu;
    private readonly AuthService authService;

    public BookingService(MockDataService mockDataService, AuthService authService)
    {
        duLieu = mockDataService;
        this.authService = authService;
    }

    // Lấy danh sách yêu cầu đăng ký
    // Nếu là Cán bộ / Chiến sĩ: chỉ lấy yêu cầu của chính mình
    // Nếu là Lãnh đạo, Đội xe, Admin: lấy toàn bộ danh sách
    public List<Booking> GetAll()
    {
        List<Booking> ketQua = new List<Booking>();
        User? nguoiDungHienTai = authService.CurrentUser;

        if (nguoiDungHienTai == null)
        {
            return ketQua;
        }

        bool laCanBo = nguoiDungHienTai.Role == UserRole.Officer;

        foreach (Booking booking in duLieu.Bookings)
        {
            if (laCanBo)
            {
                if (booking.RequesterId == nguoiDungHienTai.Id)
                {
                    ketQua.Add(SaoChep(booking));
                }
            }
            else
            {
                ketQua.Add(SaoChep(booking));
            }
        }

        return ketQua;
    }

    // Lấy danh sách các yêu cầu đang chờ lãnh đạo phê duyệt
    public List<Booking> GetPendingApprovals()
    {
        List<Booking> ketQua = new List<Booking>();

        foreach (Booking booking in duLieu.Bookings)
        {
            if (booking.Status == BookingStatus.PendingApproval)
            {
                ketQua.Add(SaoChep(booking));
            }
        }

        return ketQua;
    }

    // Lấy lịch sử các yêu cầu đã được xử lý duyệt hoặc từ chối
    public List<Booking> GetApprovalHistory()
    {
        List<Booking> ketQua = new List<Booking>();

        foreach (Booking booking in duLieu.Bookings)
        {
            if (booking.Status != BookingStatus.Draft && booking.Status != BookingStatus.PendingApproval)
            {
                ketQua.Add(SaoChep(booking));
            }
        }

        return ketQua;
    }

    // Tạo mới một yêu cầu đăng ký phương tiện
    public bool Create(Booking booking, bool guiDuyetNgay, out string loi)
    {
        User? nguoiDung = authService.CurrentUser;

        if (nguoiDung == null)
        {
            loi = "Vui lòng đăng nhập để thực hiện chức năng này.";
            return false;
        }

        loi = KiemTraDuLieu(booking);

        if (loi != "")
        {
            return false;
        }

        // Cấp mã Id tự tăng
        int idMoi = 1;

        foreach (Booking item in duLieu.Bookings)
        {
            if (item.Id >= idMoi)
            {
                idMoi = item.Id + 1;
            }
        }

        booking.Id = idMoi;
        booking.BookingCode = $"YC-{DateTime.Today.Year}-{idMoi:D3}";
        booking.RequesterId = nguoiDung.Id;
        booking.DepartmentId = nguoiDung.DepartmentId;
        booking.Status = guiDuyetNgay ? BookingStatus.PendingApproval : BookingStatus.Draft;
        booking.RejectionReason = "";

        duLieu.Bookings.Add(SaoChep(booking));

        // Ghi nhật ký hoạt động
        GhiNhatKy(nguoiDung.Id,
            guiDuyetNgay ? "Gửi yêu cầu phê duyệt" : "Tạo bản nháp yêu cầu",
            $"Cán bộ {nguoiDung.FullName} đã tạo yêu cầu {booking.BookingCode} đi {booking.Destination}.");

        return true;
    }

    // Chỉnh sửa yêu cầu (chỉ được sửa khi đang ở trạng thái Nháp hoặc Chờ phê duyệt)
    public bool Update(Booking booking, bool guiDuyet, out string loi)
    {
        User? nguoiDung = authService.CurrentUser;

        if (nguoiDung == null)
        {
            loi = "Vui lòng đăng nhập để thực hiện.";
            return false;
        }

        loi = KiemTraDuLieu(booking);

        if (loi != "")
        {
            return false;
        }

        for (int i = 0; i < duLieu.Bookings.Count; i++)
        {
            if (duLieu.Bookings[i].Id == booking.Id)
            {
                Booking donGoc = duLieu.Bookings[i];

                // Chỉ người tạo hoặc Admin mới được sửa
                if (donGoc.RequesterId != nguoiDung.Id && nguoiDung.Role != UserRole.Administrator)
                {
                    loi = "Bạn chỉ có thể chỉnh sửa yêu cầu do chính mình tạo.";
                    return false;
                }

                // Chỉ cho phép sửa khi là Nháp hoặc Chờ duyệt
                if (donGoc.Status != BookingStatus.Draft && donGoc.Status != BookingStatus.PendingApproval)
                {
                    loi = "Chỉ có thể sửa yêu cầu khi ở trạng thái Bản nháp hoặc Chờ phê duyệt.";
                    return false;
                }

                if (guiDuyet)
                {
                    booking.Status = BookingStatus.PendingApproval;
                }
                else
                {
                    booking.Status = donGoc.Status;
                }

                booking.BookingCode = donGoc.BookingCode;
                booking.RequesterId = donGoc.RequesterId;
                booking.DepartmentId = donGoc.DepartmentId;
                booking.RejectionReason = "";

                duLieu.Bookings[i] = SaoChep(booking);

                GhiNhatKy(nguoiDung.Id, "Cập nhật yêu cầu",
                    $"Cán bộ {nguoiDung.FullName} đã cập nhật yêu cầu {booking.BookingCode}.");

                return true;
            }
        }

        loi = "Không tìm thấy yêu cầu cần cập nhật.";
        return false;
    }

    // Gửi phê duyệt từ bản nháp
    public bool SendApproval(int bookingId, out string loi)
    {
        User? nguoiDung = authService.CurrentUser;

        if (nguoiDung == null)
        {
            loi = "Vui lòng đăng nhập.";
            return false;
        }

        for (int i = 0; i < duLieu.Bookings.Count; i++)
        {
            if (duLieu.Bookings[i].Id == bookingId)
            {
                if (duLieu.Bookings[i].Status != BookingStatus.Draft)
                {
                    loi = "Chỉ có thể gửi phê duyệt cho yêu cầu đang ở trạng thái Bản nháp.";
                    return false;
                }

                duLieu.Bookings[i].Status = BookingStatus.PendingApproval;

                GhiNhatKy(nguoiDung.Id, "Gửi duyệt yêu cầu",
                    $"Đã gửi yêu cầu {duLieu.Bookings[i].BookingCode} đến lãnh đạo phê duyệt.");

                loi = "";
                return true;
            }
        }

        loi = "Không tìm thấy yêu cầu.";
        return false;
    }

    // Hủy yêu cầu (áp dụng cho đơn nháp hoặc đơn đang chờ duyệt)
    public bool Cancel(int bookingId, out string loi)
    {
        User? nguoiDung = authService.CurrentUser;

        if (nguoiDung == null)
        {
            loi = "Vui lòng đăng nhập.";
            return false;
        }

        for (int i = 0; i < duLieu.Bookings.Count; i++)
        {
            if (duLieu.Bookings[i].Id == bookingId)
            {
                Booking don = duLieu.Bookings[i];

                if (don.RequesterId != nguoiDung.Id && nguoiDung.Role != UserRole.Administrator)
                {
                    loi = "Bạn chỉ có thể hủy yêu cầu của chính mình.";
                    return false;
                }

                if (don.Status != BookingStatus.Draft && don.Status != BookingStatus.PendingApproval)
                {
                    loi = "Không thể hủy yêu cầu đã được duyệt hoặc đang thực hiện.";
                    return false;
                }

                don.Status = BookingStatus.Cancelled;

                GhiNhatKy(nguoiDung.Id, "Hủy yêu cầu",
                    $"Cán bộ {nguoiDung.FullName} đã hủy yêu cầu {don.BookingCode}.");

                loi = "";
                return true;
            }
        }

        loi = "Không tìm thấy yêu cầu cần hủy.";
        return false;
    }

    // Lãnh đạo phê duyệt yêu cầu
    public bool Approve(int bookingId, out string loi)
    {
        if (!authService.CanApproveBookings)
        {
            loi = "Bạn không có quyền phê duyệt yêu cầu sử dụng phương tiện.";
            return false;
        }

        User nguoiDung = authService.CurrentUser!;

        for (int i = 0; i < duLieu.Bookings.Count; i++)
        {
            if (duLieu.Bookings[i].Id == bookingId)
            {
                if (duLieu.Bookings[i].Status != BookingStatus.PendingApproval)
                {
                    loi = "Yêu cầu này không ở trạng thái chờ phê duyệt.";
                    return false;
                }

                // Sau khi lãnh đạo duyệt, chuyển sang trạng thái Chờ điều động
                duLieu.Bookings[i].Status = BookingStatus.PendingDispatch;
                duLieu.Bookings[i].RejectionReason = "";

                GhiNhatKy(nguoiDung.Id, "Phê duyệt yêu cầu",
                    $"Lãnh đạo {nguoiDung.FullName} đã phê duyệt yêu cầu {duLieu.Bookings[i].BookingCode}. Chuyển sang chờ điều động.");

                loi = "";
                return true;
            }
        }

        loi = "Không tìm thấy yêu cầu cần phê duyệt.";
        return false;
    }

    // Lãnh đạo từ chối yêu cầu (bắt buộc nhập lý do)
    public bool Reject(int bookingId, string lyDo, out string loi)
    {
        if (!authService.CanApproveBookings)
        {
            loi = "Bạn không có quyền từ chối yêu cầu.";
            return false;
        }

        lyDo = lyDo.Trim();

        if (string.IsNullOrEmpty(lyDo))
        {
            loi = "Vui lòng nhập lý do từ chối yêu cầu.";
            return false;
        }

        User nguoiDung = authService.CurrentUser!;

        for (int i = 0; i < duLieu.Bookings.Count; i++)
        {
            if (duLieu.Bookings[i].Id == bookingId)
            {
                if (duLieu.Bookings[i].Status != BookingStatus.PendingApproval)
                {
                    loi = "Yêu cầu này không ở trạng thái chờ phê duyệt.";
                    return false;
                }

                duLieu.Bookings[i].Status = BookingStatus.Rejected;
                duLieu.Bookings[i].RejectionReason = lyDo;

                GhiNhatKy(nguoiDung.Id, "Từ chối yêu cầu",
                    $"Lãnh đạo {nguoiDung.FullName} đã từ chối yêu cầu {duLieu.Bookings[i].BookingCode}. Lý do: {lyDo}");

                loi = "";
                return true;
            }
        }

        loi = "Không tìm thấy yêu cầu cần từ chối.";
        return false;
    }

    // Lấy tên người tạo yêu cầu
    public string GetRequesterName(int requesterId)
    {
        foreach (User user in duLieu.Users)
        {
            if (user.Id == requesterId)
            {
                return user.FullName;
            }
        }

        return "Không xác định";
    }

    // Kiểm tra tính hợp lệ của dữ liệu yêu cầu
    private string KiemTraDuLieu(Booking booking)
    {
        booking.Purpose = booking.Purpose.Trim();
        booking.Departure = booking.Departure.Trim();
        booking.Destination = booking.Destination.Trim();
        booking.WorkType = booking.WorkType.Trim();

        if (string.IsNullOrEmpty(booking.Purpose))
        {
            return "Vui lòng nhập mục đích sử dụng phương tiện.";
        }

        if (string.IsNullOrEmpty(booking.Departure))
        {
            return "Vui lòng nhập địa điểm xuất phát.";
        }

        if (string.IsNullOrEmpty(booking.Destination))
        {
            return "Vui lòng nhập địa điểm đến.";
        }

        if (booking.PassengerCount <= 0)
        {
            return "Số lượng người tham gia phải lớn hơn 0.";
        }

        // Kiểm tra thời gian khởi hành không được trong quá khứ quá 30 phút (trừ trường hợp ghi nhận bù)
        if (booking.DepartureTime < DateTime.Now.AddMinutes(-30))
        {
            return "Thời gian khởi hành không được chọn trong quá khứ.";
        }

        // Thời gian trở về phải sau thời gian khởi hành tối thiểu 15 phút
        if (booking.ReturnTime <= booking.DepartureTime)
        {
            return "Thời gian trở về dự kiến phải sau thời gian khởi hành.";
        }

        if ((booking.ReturnTime - booking.DepartureTime).TotalMinutes < 15)
        {
            return "Thời gian công tác dự kiến phải tối thiểu từ 15 phút trở lên.";
        }

        return "";
    }

    // Ghi nhật ký vào danh sách In-Memory
    private void GhiNhatKy(int userId, string action, string description)
    {
        int idMoi = 1;

        foreach (ActivityLog log in duLieu.ActivityLogs)
        {
            if (log.Id >= idMoi)
            {
                idMoi = log.Id + 1;
            }
        }

        duLieu.ActivityLogs.Add(new ActivityLog
        {
            Id = idMoi,
            UserId = userId,
            Action = action,
            Description = description,
            CreatedAt = DateTime.Now
        });
    }

    // Tạo bản sao đối tượng Booking
    private Booking SaoChep(Booking b)
    {
        return new Booking
        {
            Id = b.Id,
            BookingCode = b.BookingCode,
            RequesterId = b.RequesterId,
            DepartmentId = b.DepartmentId,
            WorkType = b.WorkType,
            Purpose = b.Purpose,
            Departure = b.Departure,
            Destination = b.Destination,
            DepartureTime = b.DepartureTime,
            ReturnTime = b.ReturnTime,
            PassengerCount = b.PassengerCount,
            RequestedVehicleType = b.RequestedVehicleType,
            Status = b.Status,
            RejectionReason = b.RejectionReason
        };
    }
}
