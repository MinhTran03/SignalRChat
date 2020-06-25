using Microsoft.AspNetCore.Components;
using SignalRChat.Shared;
using System.Collections.Generic;

namespace SignalRChat.Clients.Blazor.Pages
{
	public class NavMenuBase : ComponentBase
	{
		protected bool collapseNavMenu = true;
		protected string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
		protected void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;

		[CascadingParameter] public List<ClientIdentity> ClientList { get; set; }
		[Parameter] public EventCallback<string> ChatWithSpecificClientCallBack { get; set; }

		protected void HandleOnClickSpecificClient(string clientId)
		{
			ChatWithSpecificClientCallBack.InvokeAsync(clientId);
		}
	}
}
