using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models;

public class Department
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đơn vị.")]
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
}
