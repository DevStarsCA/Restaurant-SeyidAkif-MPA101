using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Common.Models;

public class KapitalPaymentResult
{
    public bool IsSuccess { get; set; }
    public int? PurchaseId { get; set; }
    public string? HppUrl { get; set; }
    public string? Password { get; set; }
    public string? Secret { get; set; }
    public string? ErrorMessage { get; set; }
}
