using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRChat.Shared
{
	public class ChatClient : IAsyncDisposable
	{
		private HubConnection _hubConnection;
		private readonly string _username;
		private readonly string _hubUrl;
		private bool _isStarted;
		public event EventHandler<MessageReceivedEventArgs> MessageReceived;
		public event EventHandler<NotifyStateEventArgs> NotificationStateChange;

		public ChatClient(string username, string siteUrl)
		{
			if (string.IsNullOrWhiteSpace(username))
				throw new ArgumentNullException(nameof(username));
			if (string.IsNullOrWhiteSpace(siteUrl))
				throw new ArgumentNullException(nameof(siteUrl));

			_username = username;
			_hubUrl = siteUrl.TrimEnd('/') + HubConstant.HubPattern;
		}

		public async Task StartAsync()
		{
			if (_isStarted == false)
			{
				_hubConnection = new HubConnectionBuilder()
					.WithUrl(_hubUrl)
					.Build();

				_hubConnection.On<string, string>(HubConstant.ReceiveMessageMethod,
					(clientUsername, message) => HandlerReceiveMessage(clientUsername, message));
				_hubConnection.On<string, State>(HubConstant.NotifyStateMethod,
					(clientUsername, state) => HandlerNotifyOtherClientStateChange(clientUsername, state));

				await _hubConnection.StartAsync();
				_isStarted = true;

				await _hubConnection.SendAsync(HubConstant.NotifyStateMethod, _username);
			}
		}

		private void HandlerReceiveMessage(string clientUsername, string message)
		{
			MessageReceived?.Invoke(this, new MessageReceivedEventArgs
			{
				UserName = clientUsername,
				Message = message
			});
		}

		private void HandlerNotifyOtherClientStateChange(string clientUsername, State state)
		{
			NotificationStateChange?.Invoke(this, new NotifyStateEventArgs
			{
				Username = clientUsername,
				State = state
			});
		}

		public async Task SendAsync(string message)
		{
			if (_isStarted == false)
				throw new InvalidOperationException("Client not started");
			await _hubConnection.SendAsync(HubConstant.SendMessageMethod, _username, message);
		}

		public async Task StopAsync()
		{
			if (_isStarted)
			{
				await _hubConnection.StopAsync();
				await _hubConnection.DisposeAsync();
				_hubConnection = null;
				_isStarted = false;
			}
		}

		public async ValueTask DisposeAsync()
		{
			await StopAsync();
		}
	}
}
