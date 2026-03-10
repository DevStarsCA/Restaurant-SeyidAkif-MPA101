using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiAssistantController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ICategoryService _categoryService;

    public AiAssistantController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ICategoryService categoryService)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _categoryService = categoryService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] AiAssistantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { success = false, message = "Mesaj boş ola bilməz." });

        var apiKey = _configuration["Groq:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return StatusCode(500, new { success = false, message = "Groq API açarı tapılmadı." });

        var menuText = await GetMenuTextAsync();

        var systemPrompt = $@"Sən restoran saytının AI köməkçisisən. Adın RestoBot.

Qaydalar:
- Yalnız aşağıdakı məlumatlara əsasən cavab ver.
- Heç nə uydurma.
- Əgər məlumat yoxdursa belə de: ""Bu məlumat hazırda sistemdə yoxdur.""
- Cavabı qısa və aydın ver.
- Azərbaycan dilində cavab ver.

Restoran məlumatları:
- Ad: Restoran
- Ünvan: Bakı, Nizami küç. 123
- Telefon: +994 50 555 55 55
- İş saatları: Hər gün 10:00 - 23:00
- Rezervasiya: Saytda rezervasiya bölməsindən tarix, saat və qonaq sayını seçərək rezervasiya yaradıla bilər.

Menyu:
{menuText}";

        var body = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = systemPrompt
                },
                new
                {
                    role = "user",
                    content = request.Message
                }
            },
            temperature = 0.2,
            max_tokens = 300
        };

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(
            "https://api.groq.com/openai/v1/chat/completions",
            content);

        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                success = false,
                message = "AI sorğusunda xəta baş verdi.",
                details = responseText
            });
        }

        using var document = JsonDocument.Parse(responseText);
        var root = document.RootElement;

        string reply = "Cavab alınmadı.";

        if (root.TryGetProperty("choices", out var choices) &&
            choices.ValueKind == JsonValueKind.Array &&
            choices.GetArrayLength() > 0)
        {
            var firstChoice = choices[0];

            if (firstChoice.TryGetProperty("message", out var messageElement) &&
                messageElement.TryGetProperty("content", out var contentElement) &&
                contentElement.ValueKind == JsonValueKind.String)
            {
                reply = contentElement.GetString() ?? "Cavab alınmadı.";
            }
        }

        return Ok(new { success = true, reply });
    }

    private async Task<string> GetMenuTextAsync()
    {
        try
        {
            var result = await _categoryService.GetWithProductsAsync();
            if (!result.Success || result.Data == null)
                return "Menyu məlumatı yoxdur.";

            var sb = new StringBuilder();

            foreach (var cat in result.Data)
            {
                sb.AppendLine($"Kateqoriya: {cat.Name}");

                if (cat.Products != null)
                {
                    foreach (var p in cat.Products)
                    {
                        sb.AppendLine($"- {p.Name} | {p.Price:0.00} AZN | {p.Description}");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
        catch
        {
            return "Menyu məlumatı alınmadı.";
        }
    }
}

public class AiAssistantRequest
{
    public string Message { get; set; } = string.Empty;
}
