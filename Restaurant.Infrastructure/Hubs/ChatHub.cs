using Microsoft.AspNetCore.SignalR;

namespace Restaurant.Infrastructure.Hubs;

public class ChatHub : Hub
{
    public async Task JoinTableChat(string tableId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_Table_{tableId}");
    }

    public async Task JoinWaiterChat(string waiterId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_Waiter_{waiterId}");
    }

    public async Task SendMessageToWaiter(string tableId, string message)
    {
        await Clients.Group($"Chat_Table_{tableId}").SendAsync("ReceiveMessage", tableId, message, true);
    }

    public async Task SendMessageToTable(string tableId, string message)
    {
        await Clients.Group($"Chat_Table_{tableId}").SendAsync("ReceiveMessage", tableId, message, false);
    }
}