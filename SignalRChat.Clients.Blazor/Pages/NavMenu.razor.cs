using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SignalRChat.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRChat.Clients.Blazor.Pages
{
	public class NavMenuBase : ComponentBase
	{
		protected bool collapseNavMenu = true;
		protected string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
		protected void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;

		[Inject] public IJSRuntime JSRuntime { get; set; }
		[CascadingParameter] public List<ClientIdentity> ClientList { get; set; }
		[CascadingParameter] public List<GroupIdentity> GroupList { get; set; }
		[Parameter] public EventCallback<string> ChatWithSpecificClientCallBack { get; set; }
		[Parameter] public EventCallback<string> ChatWithSpecificGroupCallBack { get; set; }

		protected async Task HandleOnClickSpecificClient(string clientId)
		{
			await ChatWithSpecificClientCallBack.InvokeAsync(clientId);
			await JSRuntime.InvokeAsync<string>("activeNavClient", clientId);
		}

		protected async Task HandleOnClickSpecificGroup(string groupId)
		{
			await ChatWithSpecificGroupCallBack.InvokeAsync(groupId);
			await JSRuntime.InvokeAsync<string>("activeNavClient", groupId);
		}
	}
}
