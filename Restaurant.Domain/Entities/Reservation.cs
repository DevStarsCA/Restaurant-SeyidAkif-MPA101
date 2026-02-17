using Restaurant.Domain.Entities.Common;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities;

public class Reservation : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public DateTime ReservationDate { get; set; }            
    public int GuestCount { get; set; }                      
    public string? Note { get; set; }                        
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public Guid TableId { get; set; }
    public Table Table { get; set; } = null!;

    
    //public void Confirm()
    //{
    //    if (Status != ReservationStatus.Pending)
    //        throw new InvalidOperationException("Yalnız gözləyən rezervasiyalar təsdiqlənə bilər.");

    //    Status = ReservationStatus.Confirmed;
    //}

    //public void Cancel()
    //{
    //    if (Status == ReservationStatus.Completed)
    //        throw new InvalidOperationException("Tamamlanmış rezervasiyalar ləğv edilə bilməz.");

    //    Status = ReservationStatus.Cancelled;
    //}

    //public void Complete()
    //{
    //    if (Status != ReservationStatus.Confirmed)
    //        throw new InvalidOperationException("Yalnız təsdiqlənmiş rezervasiyalar tamamlana bilər.");

    //    Status = ReservationStatus.Completed;
    //}
}
