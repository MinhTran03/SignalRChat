using Microsoft.AspNetCore.SignalR;
using SignalRChat.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRChat.Server.Hubs
{
	public class ChatHub : Hub
	{
      private static readonly Dictionary<string, string> userLookup = 
         new Dictionary<string, string>();

      public async Task SendMessage(string user, string message)
		{
			await Clients.Others.SendAsync(HubConstant.ReceiveMessageMethod, user, message);
		}

      [HubMethodName(HubConstant.NotifyStateMethod)]
      public async Task Register(string username)
      {
         var currentId = Context.ConnectionId;
         if (!userLookup.ContainsKey(currentId))
         {
            userLookup.Add(currentId, username);
            await Clients.Others.SendAsync(HubConstant.NotifyStateMethod, username, State.Register);
         }
      }

      public override async Task OnDisconnectedAsync(Exception e)
      {
         string id = Context.ConnectionId;
         if (!userLookup.TryGetValue(id, out string username))
            username = "[unknown]";

         userLookup.Remove(id);
         await Clients.AllExcept(Context.ConnectionId).SendAsync(
             HubConstant.NotifyStateMethod,
             username, State.Disconnect);
         await base.OnDisconnectedAsync(e);
      }
   }
}
