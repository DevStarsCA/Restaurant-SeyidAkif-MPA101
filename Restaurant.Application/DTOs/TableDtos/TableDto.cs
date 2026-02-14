using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.TableDTOs;

public class TableDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string QRCode { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public TableStatus Status { get; set; }
    public string StatusText => Status.ToString();
}
