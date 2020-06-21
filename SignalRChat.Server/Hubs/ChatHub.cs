using Microsoft.AspNetCore.SignalR;
using SignalRChat.Shared;
using System.Threading.Tasks;

namespace SignalRChat.Server.Hubs
{
	public class ChatHub : Hub
	{
		public async Task SendMessage(string user, string message)
		{
			await Clients.Others.SendAsync(HubConstant.ReceiveMessageMethod, user, message);
		}
	}
}
