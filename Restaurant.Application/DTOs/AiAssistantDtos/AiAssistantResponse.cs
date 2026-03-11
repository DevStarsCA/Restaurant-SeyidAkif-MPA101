using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOs.AiAssistantDtos
{
    public class AiAssistantResponse
    {
        public bool Success { get; set; }
        public string? Reply { get; set; }
        public string? Message { get; set; }
    }
}
