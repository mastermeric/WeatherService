using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weather.WebAPI.Hubs
{
    public class NotificationHub:Hub
    {

        public async Task BroadcastChartData() => 
            await Clients.All.SendAsync("broadcastchartdata", "OKKEEYYY");


        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessageFromServerHub", "Server says :" + this.Context.ConnectionId +  " is conencted now.\n");
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.All.SendAsync("ReceiveMessageFromServerHub", "Server says :" + this.Context.ConnectionId + " is disconencted !\n");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
