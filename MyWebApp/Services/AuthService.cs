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

    public bool DaDangNhap
    {
        get
        {
            return CurrentUser != null;
        }
    }

    // Kiểm tra đăng nhập, trả về thông báo lỗi nếu sai.
    public string DangNhap(string tenDangNhap, string matKhau)
    {
        tenDangNhap = tenDangNhap.Trim();

        foreach (User user in duLieu.Users)
        {
            if (string.Equals(user.Username, tenDangNhap, StringComparison.OrdinalIgnoreCase))
            {
                if (user.Password != matKhau)
                {
                    return "Mật khẩu không đúng.";
                }

                CurrentUser = user;
                UserChanged?.Invoke();
                return "";
            }
        }

        return "Tên đăng nhập không tồn tại.";
    }

    public void DangXuat()
    {
        CurrentUser = null;
        UserChanged?.Invoke();
    }

    // --- Kiểm tra quyền ---

    public bool CanManageVehicles
    {
        get
        {
            if (CurrentUser == null)
            {
                return false;
            }

            return CurrentUser.Role == UserRole.FleetManager || CurrentUser.Role == UserRole.Administrator;
        }
    }

    public bool CanApproveBookings
    {
        get
        {
            if (CurrentUser == null)
            {
                return false;
            }

            return CurrentUser.Role == UserRole.Leader || CurrentUser.Role == UserRole.Administrator;
        }
    }

    public bool CanDispatch
    {
        get
        {
            if (CurrentUser == null)
            {
                return false;
            }

            return CurrentUser.Role == UserRole.FleetManager || CurrentUser.Role == UserRole.Administrator;
        }
    }

    // --- Hàm tiện ích ---

    public string GetRoleName(UserRole role)
    {
        return EnumHelper.TenUserRole(role);
    }
}
