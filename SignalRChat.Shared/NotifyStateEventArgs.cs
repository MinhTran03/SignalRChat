namespace SignalRChat.Shared
{
	public enum State
	{
		Register,
		Disconnect
	}

	public class NotifyStateEventArgs
	{
		public State State { get; set; }
		public ClientIdentity ClientIdentity { get; set; }
	}
}
