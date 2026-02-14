using Restaurant.Application.Common;
using Restaurant.Application.DTOs.BasketItemDtos;

namespace Restaurant.Application.Interfaces;

public interface IBasketService
{
    Task<ApiResponse<BasketSummaryDto>> GetByTableAsync(Guid tableId);
    Task<ApiResponse<BasketItemDto>> AddToBasketAsync(AddToBasketDto dto);
    Task<ApiResponse<BasketItemDto>> UpdateQuantityAsync(UpdateBasketItemDto dto);
    Task<ApiResponse<bool>> RemoveItemAsync(Guid id);
    Task<ApiResponse<bool>> ClearBasketAsync(Guid tableId);
}
