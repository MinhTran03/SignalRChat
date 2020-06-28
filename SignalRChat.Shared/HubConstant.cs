namespace SignalRChat.Shared
{
	public static class HubConstant
	{
		public const string HubUrl = "http://localhost:5000";
		public const string HubPattern = "/chat";
		public const string SendMessageToAllMethod = "SendMessageToAll";
		public const string ReceiveMessageMethod = "ReceiveMessage";
		public const string NotifyStateMethod = "NotifyState";
		public const string SendMessageToSpecificClientMethod = "SendMessageToSpecificClient";
		public const string SendMessageToGroupMethod = "SendMessageToGroup";
		public const string CreateGroupMethod = "CreateGroup";
		public const string NotifyAddedToGroupMethod = "NotifyAddedToGroup";
	}
}
