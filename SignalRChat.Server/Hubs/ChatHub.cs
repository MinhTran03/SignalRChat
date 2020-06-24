using Microsoft.AspNetCore.SignalR;
using SignalRChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Server.Hubs
{
	public class ChatHub : Hub
	{
      private static readonly List<ClientIdentity> userLookup = 
         new List<ClientIdentity>();

      public async Task SendMessage(ClientIdentity clientIdentity, string message)
		{
			await Clients.Others.SendAsync(HubConstant.ReceiveMessageMethod, clientIdentity, message);
		}

      [HubMethodName(HubConstant.NotifyStateMethod)]
      public async Task Register(ClientIdentity clientIdentity)
      {
         if (!userLookup.Exists(c => c.ClientId == clientIdentity.ClientId))
         {
            userLookup.Add(clientIdentity);
            Console.WriteLine(clientIdentity.ToString());
            await Clients.Others.SendAsync(HubConstant.NotifyStateMethod, clientIdentity, State.Register);
         }
      }

      public override async Task OnDisconnectedAsync(Exception e)
      {
         string currentId = Context.ConnectionId;
         ClientIdentity clientIdentity = userLookup.First(c => c.ClientId == currentId);
         userLookup.Remove(clientIdentity);
         Console.WriteLine(clientIdentity.ToString());
         await Clients.Others.SendAsync(
             HubConstant.NotifyStateMethod, clientIdentity, State.Disconnect);
         await base.OnDisconnectedAsync(e);
      }
   }
}
