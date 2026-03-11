using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces;

public interface IAiAssistantService
{
    Task<(bool Success, string Reply, string? ErrorMessage, int StatusCode)> ChatAsync(string message);
}