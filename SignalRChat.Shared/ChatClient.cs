using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
		public event EventHandler<NotifyAddedToGroupEventArgs> NotificationAddedToGroup;

		public string CLientId { get => _clientIdentity.ClientId; }

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
				_hubConnection.On<GroupIdentity, ClientIdentity>(HubConstant.NotifyAddedToGroupMethod,
					(groupIdentity, clientIdentity) => HandlerNotifyAddedToGroup(groupIdentity, clientIdentity));

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

		private void HandlerNotifyAddedToGroup(GroupIdentity groupIdentity, ClientIdentity clientIdentity)
		{
			NotificationAddedToGroup?.Invoke(this, new NotifyAddedToGroupEventArgs()
			{
				ClientAdded = clientIdentity,
				GroupIdentity = groupIdentity
			});
		}

		public async Task SendAsync(string message)
		{
			if (_isStarted == false)
				throw new InvalidOperationException("Client not started");
			await _hubConnection.SendAsync(HubConstant.SendMessageToAllMethod, _clientIdentity, message);
		}

		public async Task SendAsync(string message, string otherClientId)
		{
			if (_isStarted == false)
				throw new InvalidOperationException("Client not started");
			await _hubConnection.SendAsync(HubConstant.SendMessageToSpecificClientMethod,
				_clientIdentity, message, otherClientId);
		}

		public async Task SendToGroupAsync(string message, string groupId)
		{
			if (_isStarted == false)
				throw new InvalidOperationException("Client not started");
			await _hubConnection.SendAsync(HubConstant.SendMessageToGroupMethod,
				_clientIdentity, message, groupId);
		}

		/// <summary>
		/// Create Group method
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="memberIdList">Include caller</param>
		/// <returns></returns>
		public async Task CreateGroup(string groupName, IEnumerable<string> memberIdList)
		{
			if (_isStarted == false)
				throw new InvalidOperationException("Client not started");
			await _hubConnection.SendAsync(HubConstant.CreateGroupMethod,
				_clientIdentity, groupName, memberIdList);
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
