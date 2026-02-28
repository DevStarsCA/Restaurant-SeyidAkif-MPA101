using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.WaiterDtos
{
    public class WaiterTableDto
    {
        public Guid TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
