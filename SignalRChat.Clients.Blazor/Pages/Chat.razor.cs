using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SignalRChat.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRChat.Clients.Blazor.Pages
{
	public class ChatBase : ComponentBase
	{
		protected ChatClient chatClient;
		protected Dictionary<string, List<MessageReceivedEventArgs>> chatMessages;
		protected List<ClientIdentity> clientList;
		protected ClientIdentity myIdentity;
		protected string myChatMessage = string.Empty;
		protected string currentOtherClientId = string.Empty;
		protected bool isChatWithGroup = false;

		protected override void OnInitialized()
		{
			clientList = new List<ClientIdentity>();
			myIdentity = new ClientIdentity();
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

		protected async Task HandleSendMessage(KeyboardEventArgs arg = null)
		{
			if(arg == null || arg.Code == "Enter")
			{
				if (!(String.IsNullOrEmpty(currentOtherClientId) && String.IsNullOrEmpty(myChatMessage)))
				{
					await chatClient.SendAsync(myChatMessage, currentOtherClientId);
					var messageSendArg = new MessageReceivedEventArgs()
					{
						ClientIdentity = myIdentity,
						Message = myChatMessage
					};
					if (chatMessages.ContainsKey(currentOtherClientId))
					{
						chatMessages[currentOtherClientId].Add(messageSendArg);
					}
					else
					{
						chatMessages.Add(currentOtherClientId,
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
					await chatClient.StartAsync();
					myIdentity.ClientId = chatClient.CLientId;
				}
			}
		}

		protected void ChatWithSpecificClient(string clientId)
		{
			currentOtherClientId = clientId;
		}
	}
}
