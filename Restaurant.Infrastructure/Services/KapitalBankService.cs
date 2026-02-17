using Microsoft.Extensions.Configuration;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.Common.Models;
using Restaurant.Domain.Enums;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class KapitalBankService : IKapitalBankService
{
    private readonly HttpClient _httpClient;
    private readonly string _username;
    private readonly string _password;
    private readonly string _baseUrl;

    public KapitalBankService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("KapitalBank");
        _username = configuration["KapitalBank:Username"]!;
        _password = configuration["KapitalBank:Password"]!;
        _baseUrl = configuration["KapitalBank:BaseUrl"] ?? "https://e-commerce.kapitalbank.az";
    }

    public async Task<KapitalPaymentResult> CreatePaymentAsync(decimal amount, string currency, string description, string redirectUrl)
    {
        try
        {
            SetAuthHeader();

            var requestBody = new
            {
                order = new
                {
                    typeRid = "Purchase",
                    amount = amount,
                    currency = currency,
                    description = description,
                    hppRedirectUrl = redirectUrl
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/order", requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new KapitalPaymentResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Kapital Bank xətası: {response.StatusCode} - {responseContent}"
                };
            }

            var result = JsonSerializer.Deserialize<KapitalOrderResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new KapitalPaymentResult
            {
                IsSuccess = true,
                PurchaseId = result?.Order?.Id,
                HppUrl = result?.Order?.HppUrl,
                Password = result?.Order?.Password,
                Secret = result?.Order?.Secret
            };
        }
        catch (Exception ex)
        {
            return new KapitalPaymentResult
            {
                IsSuccess = false,
                ErrorMessage = $"Xəta: {ex.Message}"
            };
        }
    }

    public async Task<KapitalPaymentDetail> GetPaymentInfoAsync(int purchaseId, string password)
    {
        try
        {
            SetAuthHeader();

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/order/{purchaseId}?password={password}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new KapitalPaymentDetail
                {
                    IsSuccess = false,
                    ErrorMessage = $"Status yoxlama xətası: {response.StatusCode}"
                };
            }

            var result = JsonSerializer.Deserialize<KapitalOrderDetailResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var status = MapKapitalStatus(result?.Order?.Status ?? "");

            return new KapitalPaymentDetail
            {
                IsSuccess = true,
                PurchaseId = purchaseId,
                Status = status,
                Amount = result?.Order?.Amount ?? 0,
                Currency = result?.Order?.Currency
            };
        }
        catch (Exception ex)
        {
            return new KapitalPaymentDetail
            {
                IsSuccess = false,
                ErrorMessage = $"Xəta: {ex.Message}"
            };
        }
    }

    public async Task<KapitalPaymentResult> RefundPaymentAsync(int purchaseId, string password, decimal amount)
    {
        try
        {
            SetAuthHeader();

            var requestBody = new
            {
                order = new
                {
                    typeRid = "Refund",
                    amount = amount
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/order/{purchaseId}/refund?password={password}", requestBody);

            return new KapitalPaymentResult
            {
                IsSuccess = response.IsSuccessStatusCode,
                PurchaseId = purchaseId,
                ErrorMessage = response.IsSuccessStatusCode ? null : "Refund uğursuz."
            };
        }
        catch (Exception ex)
        {
            return new KapitalPaymentResult
            {
                IsSuccess = false,
                ErrorMessage = $"Xəta: {ex.Message}"
            };
        }
    }

    private void SetAuthHeader()
    {
        var authBytes = Encoding.UTF8.GetBytes($"{_username}:{_password}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
    }

    private static PaymentStatus MapKapitalStatus(string kapitalStatus)
    {
        return kapitalStatus.ToLower() switch
        {
            "fullypaid" => PaymentStatus.Completed,
            "authorized" => PaymentStatus.Authorized,
            "cancelled" => PaymentStatus.Cancelled,
            "rejected" => PaymentStatus.Rejected,
            "expired" => PaymentStatus.Expired,
            "declined" => PaymentStatus.Declined,
            "voided" => PaymentStatus.Voided,
            "refunded" => PaymentStatus.Refunded,
            _ => PaymentStatus.Pending
        };
    }
    // Kapital Bank response model-ləri
    internal class KapitalOrderResponse
    {
        public KapitalOrderInfo? Order { get; set; }
    }

    internal class KapitalOrderInfo
    {
        public int Id { get; set; }
        public string? HppUrl { get; set; }
        public string? Password { get; set; }
        public string? Secret { get; set; }
    }

    internal class KapitalOrderDetailResponse
    {
        public KapitalOrderDetailInfo? Order { get; set; }
    }

    internal class KapitalOrderDetailInfo
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }

}
