namespace SignalRChat.Shared
{
	public class GroupIdentity
	{
		public string GroupId { get; set; }
		public string GroupName { get; set; }
		public string OwnerId { get; set; }
		public override string ToString()
		{
			return $"[{GroupId}: {GroupName}]";
		}
	}
}
