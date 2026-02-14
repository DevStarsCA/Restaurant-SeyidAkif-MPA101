using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Common.Models;

public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentUrl { get; set; }
    public string? SessionId { get; set; }
    public string? ErrorMessage { get; set; }
}
