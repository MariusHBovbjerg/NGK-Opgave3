using Microsoft.AspNetCore.SignalR;
using NGK_11.Models;
using System.Threading.Tasks;

namespace NKG_11.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Measurement measurement)
        {
            await Clients.All.SendAsync("Created Measurement", "API: ",measurement);
        }
    }
}
