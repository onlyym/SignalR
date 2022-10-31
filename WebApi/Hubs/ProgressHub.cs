using Microsoft.AspNetCore.SignalR;

namespace SignalRHttps.Hubs
{
    public class ProgressHub : Hub
    {
        // 每个客户端连接后, 都会触发此方法
        public override async Task OnConnectedAsync()
        {
            //await Clients.Client(Context.ConnectionId).SendAsync("SetHubConnId", Context.ConnectionId);
            await Clients.All.SendAsync("SetHubConnId", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

    }
}
