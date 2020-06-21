using System;

namespace SignalRChat.Shared
{
	public class MessageReceivedEventArgs : EventArgs
	{
		public string UserName { get; set; }
		public string Message { get; set; }
	}
}
