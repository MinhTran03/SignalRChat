using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRChat.Shared
{
	public class ChatClient : IAsyncDisposable
	{
		private HubConnection _hubConnection;
		private readonly ClientIdentity _clientIdentity;
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

			_clientIdentity = new ClientIdentity() { Username = username };
			_hubUrl = siteUrl.TrimEnd('/') + HubConstant.HubPattern;
		}

		public async Task StartAsync()
		{
			if (_isStarted == false)
			{
				_hubConnection = new HubConnectionBuilder()
					.WithUrl(_hubUrl)
					.Build();

				_hubConnection.On<ClientIdentity, string>(HubConstant.ReceiveMessageMethod,
					(clientIdentity, message) => HandlerReceiveMessage(clientIdentity, message));
				_hubConnection.On<ClientIdentity, State>(HubConstant.NotifyStateMethod,
					(clientIdentity, state) => HandlerNotifyOtherClientStateChange(clientIdentity, state));

				await _hubConnection.StartAsync();
				_clientIdentity.ClientId = _hubConnection.ConnectionId;
				_isStarted = true;

				await _hubConnection.SendAsync(HubConstant.NotifyStateMethod, _clientIdentity);
			}
		}

		private void HandlerReceiveMessage(ClientIdentity clientIdentity, string message)
		{
			MessageReceived?.Invoke(this, new MessageReceivedEventArgs
			{
				ClientIdentity = clientIdentity,
				Message = message
			});
		}

		private void HandlerNotifyOtherClientStateChange(ClientIdentity clientIdentity, State state)
		{
			NotificationStateChange?.Invoke(this, new NotifyStateEventArgs
			{
				ClientIdentity = clientIdentity,
				State = state
			});
		}

		public async Task SendAsync(string message)
		{
			if (_isStarted == false)
				throw new InvalidOperationException("Client not started");
			await _hubConnection.SendAsync(HubConstant.SendMessageMethod, _clientIdentity, message);
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
