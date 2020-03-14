using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weather.WebAPI.Hubs
{
    public interface INotificationDispatcher
    {
        Task BroadCastMessage(string message);
    }
}
