using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Restaurant.Infrastructure.Hubs;

public class OrderHub : Hub
{
    public async Task JoinKitchen()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Kitchen");
    }

    public async Task JoinCashier()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Cashier");
    }

    public async Task JoinWaiter(string waiterId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Waiter_{waiterId}");
    }

    public async Task JoinTable(string tableId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Table_{tableId}");
    }
    public async Task JoinAdmin()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
    }

    public async Task NotifyNewOrder(string orderNumber, string tableName)
    {
        await Clients.Group("Kitchen").SendAsync("NewOrder", orderNumber, tableName);
    }

    public async Task NotifyOrderReady(string waiterId, string orderNumber, string tableName)
    {
        await Clients.Group($"Waiter_{waiterId}").SendAsync("OrderReady", orderNumber, tableName);
    }

    public async Task NotifyOrderStatusChanged(string tableId, string orderNumber, string status)
    {
        await Clients.Group($"Table_{tableId}").SendAsync("OrderStatusChanged", orderNumber, status);
    }

    public async Task NotifyTablePaymentRequest(string tableId)
    {
        await Clients.Group("Cashier").SendAsync("TablePaymentRequest", tableId);
    }
}