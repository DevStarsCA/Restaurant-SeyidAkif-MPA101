using Restaurant.Application.Common;
using Restaurant.Application.DTOs.OrderDtos;

namespace Restaurant.Application.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<OrderDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<List<OrderDto>>> GetByTableAsync(Guid tableId);
    Task<ApiResponse<List<KitchenOrderDto>>> GetKitchenOrdersAsync();
    Task<ApiResponse<List<CashierTableSummaryDto>>> GetCashierSummaryAsync();
    Task<ApiResponse<OrderDto>> CreateAsync(CreateOrderDto dto);
    Task<ApiResponse<OrderDto>> UpdateStatusAsync(UpdateOrderStatusDto dto);
}
