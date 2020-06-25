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
		protected List<ClientIdentity> ClientList;
		protected string myChatMessage = string.Empty;
		protected string username = string.Empty;
		private delegate Task SendMessageDelegate(string clientId);
		private SendMessageDelegate sendMessageDelegate;

		protected override void OnInitialized()
		{
			ClientList = new List<ClientIdentity>();
			sendMessageDelegate = Send;
		}

		private void ChatClient_NotificationStateChange(object sender, NotifyStateEventArgs e)
		{
			chatMessages.Add(new MessageReceivedEventArgs()
			{
				ClientIdentity = e.ClientIdentity,
				Message = e.State.ToString()
			});
			if(e.State == State.Register)
			{
				ClientList.Add(e.ClientIdentity);
			}
			else
			{
				ClientList.RemoveAll(c => c.ClientId == e.ClientIdentity.ClientId);
			}
			StateHasChanged();
		}

		private void ChatClient_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			chatMessages.Add(e);
			StateHasChanged();
		}

		protected async Task HandleSendMessage()
		{
			await sendMessageDelegate(null);
			myChatMessage = string.Empty;
			StateHasChanged();
		}

		protected async Task RegisUser()
		{
			chatMessages = new List<MessageReceivedEventArgs>();
			chatClient = new ChatClient($"{username}", HubConstant.HubUrl);
			chatClient.MessageReceived += ChatClient_MessageReceived;
			chatClient.NotificationStateChange += ChatClient_NotificationStateChange;
			await chatClient.StartAsync();
		}

		private async Task Send(string clientId)
		{
			await chatClient.SendAsync(myChatMessage);
		}

		protected void ChatWithSpecificClient(string clientIdN)
		{
			sendMessageDelegate = async clientId =>
			{
				await chatClient.SendAsync(myChatMessage, clientIdN);
			};
		}
	}
}
