namespace SignalRChat.Shared
{
	public class ClientIdentity
	{
		public string ClientId { get; set; }
		public string Username { get; set; }
		public override string ToString()
		{
			return $"[{ClientId}: {Username}]";
		}
	}
}
