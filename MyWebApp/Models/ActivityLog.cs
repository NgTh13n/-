namespace MyWebApp.Models;

public class ActivityLog
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = "";

    public string Description { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
