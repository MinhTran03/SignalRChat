using Microsoft.AspNetCore.Mvc.Formatters.Xml;
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
      private static readonly List<GroupIdentity> groupLookup =
         new List<GroupIdentity>();

      public async Task SendMessageToAll(ClientIdentity clientIdentity, string message)
		{
         Console.WriteLine("---------------- SendMessageToAll ----------------");
         Console.WriteLine("Client: {0}", clientIdentity.ToString());

         await Clients.Others.SendAsync(HubConstant.ReceiveMessageMethod, clientIdentity, message);
      }

      public async Task SendMessageToSpecificClient(ClientIdentity clientIdentity,
         string message, string otherClientId)
      {
         Console.WriteLine("---------------- SendMessageToSpecificClient ----------------");
         Console.WriteLine("Client: {0}", clientIdentity.ToString());
         Console.WriteLine("OtherClientId: {0}", otherClientId);

         if (userLookup.Any(u => u.ClientId == otherClientId))
         {
            await Clients.Client(otherClientId)
               .SendAsync(HubConstant.ReceiveMessageMethod, clientIdentity, message);
         }
         else
         {
            Console.WriteLine("ClientId [{0}] not exist", otherClientId);
         }
      }

      public async Task SendMessageToGroup(ClientIdentity clientIdentity,
         string message, string groupId)
      {
         Console.WriteLine("---------------- SendMessageToGroup ----------------");
         Console.WriteLine("Client: {0}", clientIdentity.ToString());
         Console.WriteLine("GroupId: {0}", groupId);

         if (groupLookup.Exists(gr => gr.GroupId == groupId))
         {
            await Clients.OthersInGroup(groupId)
               .SendAsync(HubConstant.ReceiveMessageMethod, clientIdentity, message);
         }
         else
         {
            Console.WriteLine("GroupId [{0}] not exist", groupId);
         }
      }

      public async Task CreateGroup(ClientIdentity clientIdentity, string groupName,
         IReadOnlyList<string> memberIdList)
      {
         Console.WriteLine("---------------- CreateGroup ----------------");
         Console.WriteLine("Client: {0}", clientIdentity.ToString());

         var groupIdentity = new GroupIdentity()
         {
            GroupId = Guid.NewGuid().ToString("N"),
            GroupName = groupName,
            OwnerId = clientIdentity.ClientId
         };

         foreach (var memberId in memberIdList)
         {
            await Groups.AddToGroupAsync(memberId, groupIdentity.GroupId);
         }
         await Clients.Clients(memberIdList).SendAsync(HubConstant.NotifyAddedToGroupMethod,
            groupIdentity, clientIdentity);
         groupLookup.Add(groupIdentity);
      }

      [HubMethodName(HubConstant.NotifyStateMethod)]
      public async Task Register(ClientIdentity clientIdentity)
      {
         Console.WriteLine("---------------- Register ----------------");
         Console.WriteLine("Client: {0}", clientIdentity.ToString());

         if (!userLookup.Exists(c => c.ClientId == clientIdentity.ClientId))
         {
            foreach (var onlineClient in userLookup)
            {
               // can create another method in ChatClient that receive List<CLientIdentity> onlines
               // in that just need to SendAsync once
               await Clients.Caller.SendAsync(HubConstant.NotifyStateMethod, onlineClient, State.Register);
            }
            userLookup.Add(clientIdentity);
            await Clients.Others.SendAsync(HubConstant.NotifyStateMethod, clientIdentity, State.Register);

            Console.WriteLine(clientIdentity.ToString() + " connected");
         }
      }

      public override async Task OnDisconnectedAsync(Exception e)
      {
         Console.WriteLine("---------------- OnDisconnectedAsync ----------------");
         Console.WriteLine("ClientId: {0}", Context.ConnectionId);

         string currentId = Context.ConnectionId;
         ClientIdentity clientIdentity = userLookup.First(c => c.ClientId == currentId);
         userLookup.Remove(clientIdentity);
         Console.WriteLine(clientIdentity.ToString() + " disconnected");
         await Clients.Others.SendAsync(
             HubConstant.NotifyStateMethod, clientIdentity, State.Disconnect);
         await base.OnDisconnectedAsync(e);
      }
   }
}
