using Restaurant.Domain.Entities.Common;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;

public class Table : BaseEntity
{
    public string Name { get; set; } = string.Empty;   
    public string QRCode { get; set; } = string.Empty; 
    public int Capacity { get; set; }   
    public TableStatus Status { get; set; } = TableStatus.Available;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public ICollection<WaiterTable> WaiterTables { get; set; } = new List<WaiterTable>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}






