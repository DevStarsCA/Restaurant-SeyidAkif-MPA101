using Restaurant.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Common.Interfaces;
public interface IPaymentService
{
    Task<PaymentResult> CreatePaymentAsync(decimal amount, string orderId, string description);
    Task<PaymentResult> CheckPaymentStatusAsync(string orderId, string sessionId);
    Task<PaymentResult> RefundPaymentAsync(string transactionId, decimal amount);
}
