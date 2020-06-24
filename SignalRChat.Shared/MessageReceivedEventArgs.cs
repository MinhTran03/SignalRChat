using System;

namespace SignalRChat.Shared
{
	public class MessageReceivedEventArgs : EventArgs
	{
		public ClientIdentity ClientIdentity { get; set; }
		public string Message { get; set; }
	}
}
