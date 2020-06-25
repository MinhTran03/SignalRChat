namespace SignalRChat.Shared
{
	public static class HubConstant
	{
		public const string HubUrl = "http://localhost:5000";
		public const string HubPattern = "/chat";
		public const string SendMessageMethod = "SendMessage";
		public const string ReceiveMessageMethod = "ReceiveMessage";
		public const string NotifyStateMethod = "NotifyState";
	}
}
