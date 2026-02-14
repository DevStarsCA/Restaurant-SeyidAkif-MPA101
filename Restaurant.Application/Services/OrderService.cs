using AutoMapper;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.OrderDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<OrderDto>> GetByIdAsync(Guid id)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);
        if (order == null) return ApiResponse<OrderDto>.FailResponse("Sifariş tapılmadı.");
        return ApiResponse<OrderDto>.SuccessResponse(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<List<OrderDto>>> GetByTableAsync(Guid tableId)
    {
        var orders = await _unitOfWork.Orders.GetOrdersByTableIdAsync(tableId);
        return ApiResponse<List<OrderDto>>.SuccessResponse(_mapper.Map<List<OrderDto>>(orders));
    }

    public async Task<ApiResponse<List<KitchenOrderDto>>> GetKitchenOrdersAsync()
    {
        var orders = await _unitOfWork.Orders.GetActiveOrdersAsync();
        return ApiResponse<List<KitchenOrderDto>>.SuccessResponse(_mapper.Map<List<KitchenOrderDto>>(orders));
    }

    public async Task<ApiResponse<List<CashierTableSummaryDto>>> GetCashierSummaryAsync()
    {
        var tables = await _unitOfWork.Tables.GetTablesByStatusAsync(TableStatus.Occupied);
        var result = new List<CashierTableSummaryDto>();

        foreach (var table in tables)
        {
            var unpaidOrders = await _unitOfWork.Orders.GetUnpaidOrdersByTableAsync(table.Id);
            if (unpaidOrders.Any())
            {
                var totalAmount = await _unitOfWork.Orders.GetTotalAmountByTableAsync(table.Id);
                result.Add(new CashierTableSummaryDto
                {
                    TableId = table.Id,
                    TableName = table.Name,
                    TotalAmount = totalAmount,
                    OrderCount = unpaidOrders.Count,
                    Orders = _mapper.Map<List<OrderDto>>(unpaidOrders)
                });
            }
        }

        return ApiResponse<List<CashierTableSummaryDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<OrderDto>> CreateAsync(CreateOrderDto dto)
    {
        var basketItems = await _unitOfWork.BasketItems.GetBasketItemsByTableIdAsync(dto.TableId);
        if (!basketItems.Any())
            return ApiResponse<OrderDto>.FailResponse("Səbət boşdur.");

        var order = new Order
        {
            OrderNumber = Order.GenerateOrderNumber(),
            TableId = dto.TableId,
            Note = dto.Note,
            Status = OrderStatus.Pending
        };

        foreach (var item in basketItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            });
        }

        order.CalculateTotal();
        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.BasketItems.ClearBasketByTableIdAsync(dto.TableId);

        var table = await _unitOfWork.Tables.GetByIdAsync(dto.TableId);
        if (table != null) table.Status = TableStatus.Occupied;

        await _unitOfWork.SaveChangesAsync();

        var orderWithDetails = await _unitOfWork.Orders.GetOrderWithDetailsAsync(order.Id);
        return ApiResponse<OrderDto>.SuccessResponse(_mapper.Map<OrderDto>(orderWithDetails), "Sifariş verildi.");
    }

    public async Task<ApiResponse<OrderDto>> UpdateStatusAsync(UpdateOrderStatusDto dto)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(dto.OrderId);
        if (order == null) return ApiResponse<OrderDto>.FailResponse("Sifariş tapılmadı.");

        switch (dto.Status)
        {
            case OrderStatus.Preparing: order.MarkAsPreparing(); break;
            case OrderStatus.Ready: order.MarkAsReady(); break;
            case OrderStatus.Delivered: order.MarkAsDelivered(); break;
            case OrderStatus.Cancelled: order.Cancel(); break;
            default: return ApiResponse<OrderDto>.FailResponse("Yanlış status.");
        }

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<OrderDto>.SuccessResponse(_mapper.Map<OrderDto>(order), "Status yeniləndi.");
    }
}
