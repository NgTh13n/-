using MyWebApp.Data;
using MyWebApp.Helpers;
using MyWebApp.Models;
using MyWebApp.Models.Enums;

namespace MyWebApp.Services;

public class AuthService
{
    private readonly MockDataService duLieu;

    // Null nghĩa là chưa đăng nhập.
    public User? CurrentUser { get; private set; } = null;

    // Sự kiện để layout và menu cập nhật lại khi người dùng thay đổi.
    public event Action? UserChanged;

    public AuthService(MockDataService mockDataService)
    {
        duLieu = mockDataService;
    }

    public bool DaDangNhap => CurrentUser != null;

    // Kiểm tra đăng nhập, trả về thông báo lỗi nếu sai.
    public string DangNhap(string tenDangNhap, string matKhau)
    {
        tenDangNhap = tenDangNhap.Trim();
        var user = duLieu.Users.FirstOrDefault(u => string.Equals(u.Username, tenDangNhap, StringComparison.OrdinalIgnoreCase));
        if (user == null) return "Tên đăng nhập không tồn tại.";
        if (user.Password != matKhau) return "Mật khẩu không đúng.";

        CurrentUser = user;
        UserChanged?.Invoke();
        return "";
    }

    public void DangXuat()
    {
        CurrentUser = null;
        UserChanged?.Invoke();
    }

    // Tiện ích trình diễn / chấm điểm: Đổi nhanh vai trò người dùng 1 chạm
    public void ChuyenVaiTroNhanh(UserRole role)
    {
        var targetUser = duLieu.Users.FirstOrDefault(u => u.Role == role);
        if (targetUser != null)
        {
            CurrentUser = targetUser;
            UserChanged?.Invoke();
        }
    }

    // --- Kiểm tra quyền ---
    public bool CanManageVehicles => CurrentUser?.Role is UserRole.FleetManager or UserRole.Administrator;
    // Phân quyền chuẩn CAND: Chỉ duy nhất Lãnh đạo đơn vị mới có thẩm quyền ký phê duyệt điều động phương tiện
    public bool CanApproveBookings => CurrentUser?.Role == UserRole.Leader;
    public bool CanViewApprovals => CurrentUser?.Role is UserRole.Leader or UserRole.Administrator;
    public bool CanDispatch => CurrentUser?.Role is UserRole.FleetManager or UserRole.Administrator;

    // --- Hàm tiện ích ---
    public string GetRoleName(UserRole role) => EnumHelper.TenUserRole(role);
}
