using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weather.WebAPI.Hubs
{
    public class NotificationDispatcher:INotificationDispatcher
    {

        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationDispatcher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadCastMessage(string message)
        {
            //await this._hubContext.Clients.All.SendAsync("MoveBlock", message);
            await this._hubContext.Clients.All.SendAsync("ReceiveMessageFromServerHub", message);
        }
    }
}
