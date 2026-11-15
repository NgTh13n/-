using MyWebApp.Data;
using MyWebApp.Helpers;
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

    // Lấy danh sách yêu cầu đăng ký (Cán bộ xem đơn của mình; Lãnh đạo/Đội xe/Admin xem tất cả)
    public List<Booking> GetAll()
    {
        var u = authService.CurrentUser;
        if (u == null) return new();
        int officerId = u.OfficerId ?? u.Id;
        var q = u.Role == UserRole.Officer 
            ? duLieu.Bookings.Where(b => b.RequesterId == officerId || b.RequesterId == u.Id) 
            : duLieu.Bookings;
        return q.Select(SaoChep).ToList();
    }

    // Lấy danh sách yêu cầu đang chờ lãnh đạo phê duyệt
    public List<Booking> GetPendingApprovals() =>
        duLieu.Bookings.Where(b => b.Status == BookingStatus.PendingApproval).Select(SaoChep).ToList();

    // Lấy danh sách yêu cầu cần bổ sung thông tin
    public List<Booking> GetNeedAdjustmentBookings()
    {
        var u = authService.CurrentUser;
        if (u == null) return new();
        int officerId = u.OfficerId ?? u.Id;
        return duLieu.Bookings
            .Where(b => b.Status == BookingStatus.NeedAdjustment && (u.Role != UserRole.Officer || b.RequesterId == officerId || b.RequesterId == u.Id))
            .Select(SaoChep).ToList();
    }

    // Lấy lịch sử các yêu cầu đã được xử lý (đã duyệt, đã từ chối, v.v.)
    public List<Booking> GetApprovalHistory() =>
        duLieu.Bookings.Where(b => b.Status != BookingStatus.Draft && b.Status != BookingStatus.PendingApproval).Select(SaoChep).ToList();

    public Booking? GetById(int id) =>
        duLieu.Bookings.FirstOrDefault(x => x.Id == id) is { } b ? SaoChep(b) : null;

    // Tạo mới một yêu cầu đăng ký phương tiện
    public bool Create(Booking booking, bool guiDuyetNgay, out string loi)
    {
        var nguoiDung = authService.CurrentUser;
        if (nguoiDung == null)
        {
            loi = "Vui lòng đăng nhập để thực hiện chức năng này.";
            return false;
        }

        loi = KiemTraDuLieu(booking);
        if (loi != "") return false;

        int idMoi = duLieu.Bookings.Count > 0 ? duLieu.Bookings.Max(x => x.Id) + 1 : 1;
        var phongDoi = duLieu.Departments.FirstOrDefault(d => d.Id == nguoiDung.DepartmentId);

        booking.Id = idMoi;
        booking.BookingCode = $"YC-{DateTime.Today.Year}-{idMoi:D3}";
        booking.RequesterId = nguoiDung.OfficerId ?? nguoiDung.Id;
        booking.DepartmentId = nguoiDung.DepartmentId;
        booking.DepartmentSnapshot = phongDoi?.Name ?? "Đơn vị CAND";
        booking.Status = guiDuyetNgay ? BookingStatus.PendingApproval : BookingStatus.Draft;
        booking.RejectionReason = "";
        booking.NeedAdjustmentReason = "";
        booking.CreatedAt = DateTime.Now;
        if (guiDuyetNgay) booking.SubmittedAt = DateTime.Now;

        duLieu.Bookings.Add(SaoChep(booking));

        string hanhDong = guiDuyetNgay ? "Gửi yêu cầu phê duyệt" : "Tạo bản nháp yêu cầu";
        duLieu.AddStatusHistory(
            "VehicleRequest",
            booking.Id,
            "None",
            booking.Status.ToString(),
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            $"{hanhDong}: {booking.BookingCode} đi {booking.Destination} ({EnumHelper.TenDriverMode(booking.PreferredDriverMode)})");

        GhiNhatKy(nguoiDung.Id,
            hanhDong,
            $"Cán bộ {nguoiDung.FullName} đã tạo yêu cầu {booking.BookingCode} đi {booking.Destination}.");

        return true;
    }

    // Chỉnh sửa yêu cầu (được sửa khi đang ở trạng thái Nháp, Chờ phê duyệt hoặc Yêu cầu bổ sung)
    public bool Update(Booking booking, bool guiDuyet, out string loi)
    {
        var nguoiDung = authService.CurrentUser;
        if (nguoiDung == null)
        {
            loi = "Vui lòng đăng nhập để thực hiện.";
            return false;
        }

        loi = KiemTraDuLieu(booking);
        if (loi != "") return false;

        var donGoc = duLieu.Bookings.FirstOrDefault(x => x.Id == booking.Id);
        if (donGoc == null)
        {
            loi = "Không tìm thấy yêu cầu cần cập nhật.";
            return false;
        }

        if (donGoc.RequesterId != nguoiDung.Id && nguoiDung.Role != UserRole.Administrator)
        {
            loi = "Bạn chỉ có thể chỉnh sửa yêu cầu của chính mình.";
            return false;
        }

        if (donGoc.Status != BookingStatus.Draft && 
            donGoc.Status != BookingStatus.PendingApproval && 
            donGoc.Status != BookingStatus.NeedAdjustment)
        {
            loi = "Không thể chỉnh sửa yêu cầu đã được duyệt hoặc đang thực hiện.";
            return false;
        }

        string trangThaiCu = donGoc.Status.ToString();
        donGoc.WorkType = booking.WorkType;
        donGoc.Purpose = booking.Purpose;
        donGoc.TaskDescription = booking.TaskDescription;
        donGoc.Departure = booking.Departure;
        donGoc.Destination = booking.Destination;
        donGoc.DepartureTime = booking.DepartureTime;
        donGoc.ReturnTime = booking.ReturnTime;
        donGoc.PassengerCount = booking.PassengerCount;
        donGoc.RequestedVehicleType = booking.RequestedVehicleType;
        donGoc.PreferredDriverMode = booking.PreferredDriverMode;
        donGoc.Notes = booking.Notes;

        if (guiDuyet)
        {
            donGoc.Status = BookingStatus.PendingApproval;
            donGoc.SubmittedAt = DateTime.Now;
            donGoc.NeedAdjustmentReason = "";
            donGoc.RejectionReason = "";

            duLieu.AddStatusHistory(
                "VehicleRequest",
                donGoc.Id,
                trangThaiCu,
                "PendingApproval",
                nguoiDung.OfficerId ?? nguoiDung.Id,
                nguoiDung.FullName,
                "Cán bộ đã bổ sung/chỉnh sửa thông tin và gửi lại lãnh đạo phê duyệt.");
        }

        GhiNhatKy(nguoiDung.Id, "Cập nhật yêu cầu",
            $"Cán bộ {nguoiDung.FullName} đã cập nhật yêu cầu {booking.BookingCode}.");

        return true;
    }

    // Gửi phê duyệt từ bản nháp
    public bool SendApproval(int bookingId, out string loi)
    {
        var nguoiDung = authService.CurrentUser;
        if (nguoiDung == null)
        {
            loi = "Vui lòng đăng nhập.";
            return false;
        }

        var don = duLieu.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (don == null)
        {
            loi = "Không tìm thấy yêu cầu.";
            return false;
        }

        if (don.Status != BookingStatus.Draft && don.Status != BookingStatus.NeedAdjustment)
        {
            loi = "Chỉ có thể gửi phê duyệt cho yêu cầu đang ở trạng thái Bản nháp hoặc Yêu cầu bổ sung.";
            return false;
        }

        string trangThaiCu = don.Status.ToString();
        don.Status = BookingStatus.PendingApproval;
        don.SubmittedAt = DateTime.Now;
        don.NeedAdjustmentReason = "";

        duLieu.AddStatusHistory(
            "VehicleRequest",
            bookingId,
            trangThaiCu,
            "PendingApproval",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            "Gửi yêu cầu tới Lãnh đạo phê duyệt.");

        GhiNhatKy(nguoiDung.Id, "Gửi duyệt yêu cầu",
            $"Đã gửi yêu cầu {don.BookingCode} đến lãnh đạo phê duyệt.");

        loi = "";
        return true;
    }

    // Hủy yêu cầu (áp dụng cho đơn nháp, đơn chờ duyệt hoặc đơn cần bổ sung)
    public bool Cancel(int bookingId, string lyDo, out string loi)
    {
        var nguoiDung = authService.CurrentUser;
        if (nguoiDung == null)
        {
            loi = "Vui lòng đăng nhập.";
            return false;
        }

        var don = duLieu.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (don == null)
        {
            loi = "Không tìm thấy yêu cầu cần hủy.";
            return false;
        }

        if (don.RequesterId != nguoiDung.Id && nguoiDung.Role != UserRole.Administrator)
        {
            loi = "Bạn chỉ có thể hủy yêu cầu của chính mình.";
            return false;
        }

        if (don.Status != BookingStatus.Draft && 
            don.Status != BookingStatus.PendingApproval && 
            don.Status != BookingStatus.NeedAdjustment)
        {
            loi = "Không thể hủy yêu cầu đã được duyệt hoặc đang thực hiện.";
            return false;
        }

        string trangThaiCu = don.Status.ToString();
        don.Status = BookingStatus.Cancelled;
        don.CancellationReason = lyDo;

        duLieu.AddStatusHistory(
            "VehicleRequest",
            don.Id,
            trangThaiCu,
            "Cancelled",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            string.IsNullOrWhiteSpace(lyDo) ? "Người đề nghị rút/hủy yêu cầu" : $"Hủy yêu cầu: {lyDo}");

        GhiNhatKy(nguoiDung.Id, "Hủy yêu cầu",
            $"Cán bộ {nguoiDung.FullName} đã hủy yêu cầu {don.BookingCode}.");

        loi = "";
        return true;
    }

    public bool Cancel(int bookingId, out string loi) =>
        Cancel(bookingId, "Cán bộ tự rút yêu cầu", out loi);

    // Lãnh đạo phê duyệt yêu cầu (kèm ý kiến chỉ đạo)
    public bool Approve(int bookingId, string yKienChiDao, out string loi)
    {
        if (!authService.CanApproveBookings)
        {
            loi = "Chỉ Lãnh đạo đơn vị mới có thẩm quyền phê duyệt yêu cầu sử dụng phương tiện.";
            return false;
        }

        var don = duLieu.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (don == null)
        {
            loi = "Không tìm thấy yêu cầu cần phê duyệt.";
            return false;
        }

        if (don.Status != BookingStatus.PendingApproval)
        {
            loi = "Yêu cầu này không ở trạng thái chờ phê duyệt.";
            return false;
        }

        var nguoiDung = authService.CurrentUser!;
        don.Status = BookingStatus.Approved;
        don.ApprovalOfficerId = nguoiDung.OfficerId ?? nguoiDung.Id;
        don.ApprovalAt = DateTime.Now;
        don.ApprovalComment = yKienChiDao;
        don.RejectionReason = "";
        don.NeedAdjustmentReason = "";

        string comment = string.IsNullOrWhiteSpace(yKienChiDao) 
            ? "Lãnh đạo đồng ý phê duyệt yêu cầu" 
            : $"Phê duyệt: {yKienChiDao}";

        duLieu.AddStatusHistory(
            "VehicleRequest",
            bookingId,
            "PendingApproval",
            "Approved",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            comment);

        GhiNhatKy(nguoiDung.Id, "Phê duyệt yêu cầu",
            $"Lãnh đạo {nguoiDung.FullName} đã phê duyệt yêu cầu {don.BookingCode}. Chuyển sang chờ điều động.");

        loi = "";
        return true;
    }

    public bool Approve(int bookingId, out string loi) =>
        Approve(bookingId, "Đồng ý phê duyệt, giao Đội xe điều phối.", out loi);

    // Lãnh đạo yêu cầu bổ sung / chỉnh sửa thông tin (bắt buộc nhập nội dung yêu cầu)
    public bool RequestAdjustment(int bookingId, string noiDungBoSung, out string loi)
    {
        if (!authService.CanApproveBookings)
        {
            loi = "Chỉ Lãnh đạo đơn vị mới có thẩm quyền yêu cầu bổ sung thông tin hồ sơ.";
            return false;
        }

        noiDungBoSung = (noiDungBoSung ?? "").Trim();
        if (string.IsNullOrEmpty(noiDungBoSung))
        {
            loi = "Vui lòng nhập nội dung chỉ đạo cần bổ sung / chỉnh sửa.";
            return false;
        }

        var don = duLieu.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (don == null)
        {
            loi = "Không tìm thấy yêu cầu cần xử lý.";
            return false;
        }

        if (don.Status != BookingStatus.PendingApproval)
        {
            loi = "Chỉ có thể yêu cầu bổ sung đối với yêu cầu đang chờ phê duyệt.";
            return false;
        }

        var nguoiDung = authService.CurrentUser!;
        don.Status = BookingStatus.NeedAdjustment;
        don.NeedAdjustmentReason = noiDungBoSung;
        don.ApprovalOfficerId = nguoiDung.OfficerId ?? nguoiDung.Id;
        don.ApprovalAt = DateTime.Now;

        duLieu.AddStatusHistory(
            "VehicleRequest",
            bookingId,
            "PendingApproval",
            "NeedAdjustment",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            $"Yêu cầu bổ sung: {noiDungBoSung}");

        GhiNhatKy(nguoiDung.Id, "Yêu cầu bổ sung thông tin",
            $"Lãnh đạo {nguoiDung.FullName} yêu cầu cán bộ bổ sung thông tin cho yêu cầu {don.BookingCode}. Nội dung: {noiDungBoSung}");

        loi = "";
        return true;
    }

    // Lãnh đạo từ chối yêu cầu (bắt buộc nhập lý do)
    public bool Reject(int bookingId, string lyDo, out string loi)
    {
        if (!authService.CanApproveBookings)
        {
            loi = "Chỉ Lãnh đạo đơn vị mới có thẩm quyền từ chối yêu cầu sử dụng phương tiện.";
            return false;
        }

        lyDo = (lyDo ?? "").Trim();
        if (string.IsNullOrEmpty(lyDo))
        {
            loi = "Vui lòng nhập lý do từ chối yêu cầu.";
            return false;
        }

        var don = duLieu.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (don == null)
        {
            loi = "Không tìm thấy yêu cầu cần từ chối.";
            return false;
        }

        if (don.Status != BookingStatus.PendingApproval)
        {
            loi = "Yêu cầu này không ở trạng thái chờ phê duyệt.";
            return false;
        }

        var nguoiDung = authService.CurrentUser!;
        don.Status = BookingStatus.Rejected;
        don.RejectionReason = lyDo;
        don.ApprovalOfficerId = nguoiDung.OfficerId ?? nguoiDung.Id;
        don.ApprovalAt = DateTime.Now;

        duLieu.AddStatusHistory(
            "VehicleRequest",
            bookingId,
            "PendingApproval",
            "Rejected",
            nguoiDung.OfficerId ?? nguoiDung.Id,
            nguoiDung.FullName,
            $"Từ chối: {lyDo}");

        GhiNhatKy(nguoiDung.Id, "Từ chối yêu cầu",
            $"Lãnh đạo {nguoiDung.FullName} đã từ chối yêu cầu {don.BookingCode}. Lý do: {lyDo}");

        loi = "";
        return true;
    }

    // Lấy tên người tạo yêu cầu
    public string GetRequesterName(int requesterId)
    {
        // Ưu tiên tìm theo OfficerId vì RequesterId đại diện cho cán bộ
        if (duLieu.Officers.FirstOrDefault(o => o.Id == requesterId) is { } cb)
            return cb.FullName;

        if (duLieu.Users.FirstOrDefault(u => u.Id == requesterId) is { } user)
            return user.FullName;

        return "Không xác định";
    }

    // Kiểm tra tính hợp lệ của dữ liệu yêu cầu
    private string KiemTraDuLieu(Booking booking)
    {
        booking.Purpose = (booking.Purpose ?? "").Trim();
        booking.Departure = (booking.Departure ?? "").Trim();
        booking.Destination = (booking.Destination ?? "").Trim();
        booking.WorkType = (booking.WorkType ?? "").Trim();
        booking.TaskDescription = (booking.TaskDescription ?? "").Trim();

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

        // Thời gian khởi hành không được trong quá khứ quá 30 phút
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
        int idMoi = duLieu.ActivityLogs.Count > 0 ? duLieu.ActivityLogs.Max(x => x.Id) + 1 : 1;

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
            DepartmentSnapshot = b.DepartmentSnapshot,
            WorkType = b.WorkType,
            Purpose = b.Purpose,
            TaskDescription = b.TaskDescription,
            Departure = b.Departure,
            Destination = b.Destination,
            DepartureTime = b.DepartureTime,
            ReturnTime = b.ReturnTime,
            PassengerCount = b.PassengerCount,
            RequestedVehicleType = b.RequestedVehicleType,
            PreferredDriverMode = b.PreferredDriverMode,
            Status = b.Status,
            RejectionReason = b.RejectionReason,
            NeedAdjustmentReason = b.NeedAdjustmentReason,
            ApprovalOfficerId = b.ApprovalOfficerId,
            ApprovalAt = b.ApprovalAt,
            ApprovalComment = b.ApprovalComment,
            CancellationReason = b.CancellationReason,
            CreatedAt = b.CreatedAt,
            SubmittedAt = b.SubmittedAt,
            Notes = b.Notes
        };
    }
}
