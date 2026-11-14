namespace MyWebApp.Models;

/// <summary>
/// Model thông báo hệ thống phục vụ hiển thị chuông thông báo trên thanh tiêu đề
/// </summary>
public class AppNotification
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public string Icon { get; set; } = "notifications";
    public string IconColor { get; set; } = "#1b2e4b";
    public string TargetUrl { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false;

    public string TimeDisplay
    {
        get
        {
            var diff = DateTime.Now - CreatedAt;
            if (diff.TotalMinutes < 1) return "Vừa xong";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} phút trước";
            if (diff.TotalHours < 24 && CreatedAt.Date == DateTime.Today) return $"Hôm nay, {CreatedAt:HH:mm}";
            return CreatedAt.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
