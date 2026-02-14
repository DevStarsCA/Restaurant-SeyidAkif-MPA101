using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.WaiterDtos;

public class WaiterDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string? AppUserId { get; set; }
    public List<string> AssignedTables { get; set; } = new();
}
