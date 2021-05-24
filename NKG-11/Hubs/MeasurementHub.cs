using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NKG_11.Hubs
{
    public class MeasurementHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
