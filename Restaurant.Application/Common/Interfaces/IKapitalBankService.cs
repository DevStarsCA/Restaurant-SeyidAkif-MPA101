using Restaurant.Application.Common.Models;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Common.Interfaces;

public interface IKapitalBankService
{
    Task<KapitalPaymentResult> CreatePaymentAsync(decimal amount, string currency, string description, string redirectUrl);
    Task<KapitalPaymentDetail> GetPaymentInfoAsync(int purchaseId, string password);
    Task<KapitalPaymentResult> RefundPaymentAsync(int purchaseId, string password, decimal amount);
}
