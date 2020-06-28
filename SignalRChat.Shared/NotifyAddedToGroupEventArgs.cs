namespace SignalRChat.Shared
{
	public class NotifyAddedToGroupEventArgs
	{
		public ClientIdentity ClientAdded { get; set; }
		public GroupIdentity GroupIdentity { get; set; }
	}
}
