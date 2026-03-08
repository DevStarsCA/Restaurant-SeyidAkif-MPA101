using AutoMapper;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.OrderDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces;
using System.Linq.Expressions;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<ApiResponse<List<OrderDto>>> GetAllAsync()
    {
        var orders = await _unitOfWork.Orders.GetAllOrdersWithDetailsAsync();
        return ApiResponse<List<OrderDto>>.SuccessResponse(_mapper.Map<List<OrderDto>>(orders));
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
        // Masa yoxla
        var tableCheck = await _unitOfWork.Tables.GetByIdAsync(dto.TableId);
        if (tableCheck == null)
            return ApiResponse<OrderDto>.FailResponse("Masa tapılmadı.");

        var basketItems = await _unitOfWork.BasketItems.GetBasketItemsByTableIdAsync(dto.TableId);
        if (!basketItems.Any())
            return ApiResponse<OrderDto>.FailResponse("Səbət boşdur.");

        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
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

        order.TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
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

        var validTransition = (order.Status, dto.Status) switch
        {
            (OrderStatus.Pending, OrderStatus.Preparing) => true,
            (OrderStatus.Preparing, OrderStatus.Ready) => true,
            (OrderStatus.Ready, OrderStatus.Delivered) => true,
            (OrderStatus.Delivered, OrderStatus.Completed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Preparing, OrderStatus.Cancelled) => true,
            _ => false
        };


        if (!validTransition)
            return ApiResponse<OrderDto>.FailResponse($"'{order.Status}' statusundan '{dto.Status}' statusuna keçid mümkün deyil.");

        order.Status = dto.Status;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<OrderDto>.SuccessResponse(_mapper.Map<OrderDto>(order), "Status yeniləndi.");
    }
    public async Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
        if (order == null) return ApiResponse<bool>.FailResponse("Sifariş tapılmadı.");

        if (order.Status != OrderStatus.Pending)
            return ApiResponse<bool>.FailResponse("Yalnız gözləyən sifarişlər ləğv edilə bilər.");

        order.Status = OrderStatus.Cancelled;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        await CheckAndFreeTableAsync(order.TableId);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Sifariş ləğv edildi.");
    }

    private async Task CheckAndFreeTableAsync(Guid tableId)
    {
        var unpaidOrders = await _unitOfWork.Orders.GetUnpaidOrdersByTableAsync(tableId);
        if (!unpaidOrders.Any())
        {
            var table = await _unitOfWork.Tables.GetByIdAsync(tableId);
            if (table != null) table.Status = TableStatus.Available;
        }
    }
}