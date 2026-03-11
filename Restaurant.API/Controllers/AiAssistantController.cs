using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;
using Restaurant.Application.DTOs.AiAssistantDtos;

namespace Restaurant.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiAssistantController : ControllerBase
{
    private readonly IAiAssistantService _aiAssistantService;

    public AiAssistantController(IAiAssistantService aiAssistantService)
    {
        _aiAssistantService = aiAssistantService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] AiAssistantRequest? request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                success = false,
                message = "Mesaj boş ola bilməz."
            });
        }

        var result = await _aiAssistantService.ChatAsync(request.Message);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.ErrorMessage
            });
        }

        return Ok(new AiAssistantResponse
        {
            Success = true,
            Reply = result.Reply
        });
    }
}