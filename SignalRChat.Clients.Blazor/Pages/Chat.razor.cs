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
		private ChatClient chatClient;
		protected List<MessageReceivedEventArgs> chatMessages;
		protected string myChatMessage = string.Empty;

		protected override async Task OnInitializedAsync()
		{
			chatMessages = new List<MessageReceivedEventArgs>();
			chatClient = new ChatClient($"{DateTime.Now}", "http://localhost:5000");
			chatClient.MessageReceived += ChatClient_MessageReceived;
			chatClient.NotificationStateChange += ChatClient_NotificationStateChange;
			await chatClient.StartAsync();
		}

		private void ChatClient_NotificationStateChange(object sender, NotifyStateEventArgs e)
		{
			chatMessages.Add(new MessageReceivedEventArgs()
			{
				UserName = e.Username,
				Message = e.State.ToString()
			});
		}

		private void ChatClient_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			chatMessages.Add(e);
		}

		protected async Task HandleSendMessage(MouseEventArgs args)
		{
			await chatClient.SendAsync(myChatMessage);
			myChatMessage = string.Empty;
		}
	}
}
