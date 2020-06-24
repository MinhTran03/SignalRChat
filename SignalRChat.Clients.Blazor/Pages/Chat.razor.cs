using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SignalRChat.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRChat.Clients.Blazor.Pages
{
	public class ChatBase : ComponentBase
	{
		protected ChatClient chatClient;
		protected List<MessageReceivedEventArgs> chatMessages;
		protected string myChatMessage = string.Empty;
		protected string username = string.Empty;

		private void ChatClient_NotificationStateChange(object sender, NotifyStateEventArgs e)
		{
			chatMessages.Add(new MessageReceivedEventArgs()
			{
				ClientIdentity = e.ClientIdentity,
				Message = e.State.ToString()
			});
			StateHasChanged();
		}

		private void ChatClient_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			chatMessages.Add(e);
			StateHasChanged();
		}

		protected async Task HandleSendMessage(MouseEventArgs args)
		{
			await chatClient.SendAsync(myChatMessage);
			myChatMessage = string.Empty;
			StateHasChanged();
		}

		protected async Task RegisUser()
		{
			chatMessages = new List<MessageReceivedEventArgs>();
			chatClient = new ChatClient($"{username}", "http://localhost:5000");
			chatClient.MessageReceived += ChatClient_MessageReceived;
			chatClient.NotificationStateChange += ChatClient_NotificationStateChange;
			await chatClient.StartAsync();
		}
	}
}
