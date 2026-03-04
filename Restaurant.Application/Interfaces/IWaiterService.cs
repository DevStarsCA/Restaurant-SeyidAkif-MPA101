using Microsoft.AspNetCore.Http;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.WaiterDtos;

namespace Restaurant.Application.Interfaces;

public interface IWaiterService
{
    Task<ApiResponse<List<WaiterDto>>> GetAllAsync();
    Task<ApiResponse<WaiterPanelDto>> GetWaiterPanelAsync(Guid waiterId);
    Task<ApiResponse<WaiterDto>> CreateAsync(CreateWaiterDto dto, IFormFile? image);
    Task<ApiResponse<WaiterDto>> UpdateAsync(UpdateWaiterDto dto);
    Task<ApiResponse<bool>> AssignToTableAsync(AssignWaiterToTableDto dto);
    Task<ApiResponse<bool>> UnassignFromTableAsync(AssignWaiterToTableDto dto);
}
