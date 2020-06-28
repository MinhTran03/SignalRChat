using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SignalRChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Clients.Blazor.Pages
{
	public class ChatBase : ComponentBase
	{
		protected ChatClient chatClient;
		protected Dictionary<string, List<MessageReceivedEventArgs>> chatMessages;
		protected List<ClientIdentity> clientList;
		protected List<GroupIdentity> groupList;
		protected ClientIdentity myIdentity;
		protected string myChatMessage = string.Empty;
		protected string currentRequestId = string.Empty;
		protected bool isChatWithGroup = false;

		protected override void OnInitialized()
		{
			clientList = new List<ClientIdentity>();
			groupList = new List<GroupIdentity>();

			myIdentity = new ClientIdentity();
		}

		protected async Task HandleSendMessage(KeyboardEventArgs arg = null)
		{
			if(arg == null || arg.Code == "Enter")
			{
				if (!(String.IsNullOrEmpty(currentRequestId) && String.IsNullOrEmpty(myChatMessage)))
				{
					if(!isChatWithGroup) await chatClient.SendAsync(myChatMessage, currentRequestId);
					else await chatClient.SendToGroupAsync(myChatMessage, currentRequestId);
					var messageSendArg = new MessageReceivedEventArgs()
					{
						ClientIdentity = myIdentity,
						Message = myChatMessage
					};
					if (chatMessages.ContainsKey(currentRequestId))
					{
						chatMessages[currentRequestId].Add(messageSendArg);
					}
					else
					{
						chatMessages.Add(currentRequestId,
							new List<MessageReceivedEventArgs>() { messageSendArg });
					}
					myChatMessage = string.Empty;
					StateHasChanged();
				}
			}
		}

		protected async Task RegisUser(KeyboardEventArgs arg = null)
		{
			if (arg == null || arg.Code == "Enter")
			{
				if (!String.IsNullOrWhiteSpace(myIdentity.Username))
				{
					chatMessages = new Dictionary<string, List<MessageReceivedEventArgs>>();
					chatClient = new ChatClient($"{myIdentity.Username}", HubConstant.HubUrl);
					chatClient.MessageReceived += ChatClient_MessageReceived;
					chatClient.NotificationStateChange += ChatClient_NotificationStateChange;
					chatClient.NotificationAddedToGroup += ChatClient_NotificationAddedToGroup;
					await chatClient.StartAsync();
					myIdentity.ClientId = chatClient.CLientId;
				}
			}
		}

		private void ChatClient_NotificationStateChange(object sender, NotifyStateEventArgs e)
		{
			if (e.State == State.Register)
			{
				clientList.Add(e.ClientIdentity);
			}
			else
			{
				chatMessages.Remove(e.ClientIdentity.ClientId);
				clientList.RemoveAll(c => c.ClientId == e.ClientIdentity.ClientId);
			}
			StateHasChanged();
		}

		private void ChatClient_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			if (chatMessages.ContainsKey(e.ClientIdentity.ClientId))
			{
				chatMessages[e.ClientIdentity.ClientId].Add(e);
			}
			else
			{
				chatMessages.Add(e.ClientIdentity.ClientId,
					new List<MessageReceivedEventArgs>() { e });
			}
			StateHasChanged();
		}

		private void ChatClient_NotificationAddedToGroup(object sender, NotifyAddedToGroupEventArgs e)
		{
			groupList.Add(e.GroupIdentity);
			StateHasChanged();
		}

		protected void ChatWithSpecificClient(string clientId)
		{
			isChatWithGroup = false;
			currentRequestId = clientId;
		}

		protected void ChatWithSpecificGroup(string groupId)
		{
			isChatWithGroup = true;
			currentRequestId = groupId;
		}

		protected async Task HandleCreateGroup(Dictionary<string, List<string>> args)
		{
			string groupName = args.Keys.ToList()[0];
			args[groupName].Add(myIdentity.ClientId);
			await chatClient.CreateGroup(groupName, args[groupName]);
		}
	}
}
