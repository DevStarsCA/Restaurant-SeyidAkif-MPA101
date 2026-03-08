using Microsoft.Extensions.Configuration;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.Common.Models;
using Restaurant.Domain.Enums;
using System.Net.Http.Headers;
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
        _baseUrl = configuration["KapitalBank:BaseUrl"] ?? "https://txpgtst.kapitalbank.az";
    }

    public async Task<KapitalPaymentResult> CreatePaymentAsync(decimal amount, string currency, string description, string redirectUrl)
    {
        try
        {
            var payload = new
            {
                order = new
                {
                    typeRid = "Order_SMS",
                    amount = amount.ToString("0.00").Replace(",", "."),
                    currency = currency,
                    language = "az",
                    description = description,
                    hppRedirectUrl = redirectUrl,
                    hppCofCapturePurposes = new[] { "Cit" }
                }
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(payload, options);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/order/");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var basicValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicValue);

            using var response = await _httpClient.SendAsync(request);
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

            // HppUrl-ə id və password əlavə et (Kapital Bank formatı)
            var fullHppUrl = $"{result?.Order?.HppUrl}?id={result?.Order?.Id}&password={result?.Order?.Password}";

            return new KapitalPaymentResult
            {
                IsSuccess = true,
                PurchaseId = result?.Order?.Id,
                HppUrl = fullHppUrl,
                Password = result?.Order?.Password,
                Secret = result?.Order?.Secret
            };
        }
        catch (Exception ex)
        {
            return new KapitalPaymentResult
            {
                IsSuccess = false,
                ErrorMessage = $"Kapital Bank xətası: {ex.Message}"
            };
        }
    }

    public async Task<KapitalPaymentDetail> GetPaymentInfoAsync(int purchaseId, string password)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/order/{purchaseId}");

            var basicValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicValue);

            using var response = await _httpClient.SendAsync(request);
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
            var payload = new
            {
                order = new
                {
                    typeRid = "Refund",
                    amount = amount.ToString("0.00").Replace(",", ".")
                }
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(payload, options);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/order/{purchaseId}/refund?password={password}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var basicValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicValue);

            using var response = await _httpClient.SendAsync(request);

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