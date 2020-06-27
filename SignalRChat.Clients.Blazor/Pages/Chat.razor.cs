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
		protected List<MessageReceivedEventArgs> chatMessages;
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
			//chatMessages.Add(new MessageReceivedEventArgs()
			//{
			//	ClientIdentity = e.ClientIdentity,
			//	Message = e.State.ToString()
			//});
			if (e.State == State.Register)
			{
				clientList.Add(e.ClientIdentity);
			}
			else
			{
				clientList.RemoveAll(c => c.ClientId == e.ClientIdentity.ClientId);
			}
			StateHasChanged();
		}

		private void ChatClient_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			chatMessages.Add(e);
			StateHasChanged();
		}

		protected async Task HandleSendMessage(KeyboardEventArgs arg = null)
		{
			if(arg == null || arg.Code == "Enter")
			{
				if (!(String.IsNullOrEmpty(currentOtherClientId) && String.IsNullOrEmpty(myChatMessage)))
				{
					await chatClient.SendAsync(myChatMessage, currentOtherClientId);
					chatMessages.Add(new MessageReceivedEventArgs()
					{
						ClientIdentity = myIdentity,
						Message = myChatMessage
					});
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
					chatMessages = new List<MessageReceivedEventArgs>();
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
