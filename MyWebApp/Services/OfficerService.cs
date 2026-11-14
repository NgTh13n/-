using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Services;

/// <summary>
/// Dịch vụ quản lý hồ sơ Cán bộ / Chiến sĩ trong đơn vị
/// </summary>
public class OfficerService
{
    private readonly MockDataService duLieu;
    private readonly AuthService authService;

    public OfficerService(MockDataService mockDataService, AuthService authService)
    {
        duLieu = mockDataService;
        this.authService = authService;
    }

    public List<Officer> GetAll() => duLieu.Officers.Select(SaoChep).ToList();

    public Officer? GetById(int id) =>
        duLieu.Officers.FirstOrDefault(x => x.Id == id) is { } cb ? SaoChep(cb) : null;

    /// <summary>
    /// Lấy danh sách cán bộ có quyền và đủ điều kiện tự lái xe công vụ
    /// </summary>
    public List<Officer> GetSelfDrivingOfficers() =>
        duLieu.Officers.Where(cb => cb.IsActive && cb.CanSelfDrive).Select(SaoChep).ToList();

    public bool Add(Officer officer, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền quản lý hồ sơ cán bộ.";
            return false;
        }

        loi = KiemTra(officer, 0);
        if (loi != "") return false;

        officer.Id = duLieu.Officers.Count > 0 ? duLieu.Officers.Max(x => x.Id) + 1 : 1;
        duLieu.Officers.Add(SaoChep(officer));
        return true;
    }

    public bool Update(Officer officer, out string loi)
    {
        if (!authService.CanManageVehicles)
        {
            loi = "Bạn không có quyền quản lý hồ sơ cán bộ.";
            return false;
        }

        loi = KiemTra(officer, officer.Id);
        if (loi != "") return false;

        int index = duLieu.Officers.FindIndex(o => o.Id == officer.Id);
        if (index < 0)
        {
            loi = "Không tìm thấy hồ sơ cán bộ cần cập nhật.";
            return false;
        }

        duLieu.Officers[index] = SaoChep(officer);
        return true;
    }

    private string KiemTra(Officer officer, int idBoQua)
    {
        officer.Code = officer.Code.Trim();
        officer.FullName = officer.FullName.Trim();

        if (string.IsNullOrEmpty(officer.Code))
            return "Vui lòng nhập mã cán bộ.";

        if (string.IsNullOrEmpty(officer.FullName))
            return "Vui lòng nhập họ tên cán bộ.";

        if (duLieu.Officers.Any(cb => cb.Id != idBoQua && string.Equals(cb.Code, officer.Code, StringComparison.OrdinalIgnoreCase)))
            return "Mã cán bộ này đã tồn tại.";

        return "";
    }

    private Officer SaoChep(Officer cb)
    {
        return new Officer
        {
            Id = cb.Id,
            Code = cb.Code,
            FullName = cb.FullName,
            RankOrTitle = cb.RankOrTitle,
            DepartmentId = cb.DepartmentId,
            Department = cb.Department,
            IsActive = cb.IsActive,
            CanSelfDrive = cb.CanSelfDrive,
            Phone = cb.Phone,
            Notes = cb.Notes
        };
    }
}
