using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.ReservationDtos;

public class ReservationDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public DateTime ReservationDate { get; set; }
    public int GuestCount { get; set; }
    public string? Note { get; set; }
    public ReservationStatus Status { get; set; }
    public string StatusText => Status.ToString();
    public Guid TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
}
