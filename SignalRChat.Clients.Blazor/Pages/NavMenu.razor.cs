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
		[Parameter] public EventCallback<string> ChatWithSpecificClientCallBack { get; set; }

		protected async Task HandleOnClickSpecificClient(string clientId)
		{
			await ChatWithSpecificClientCallBack.InvokeAsync(clientId);
			await JSRuntime.InvokeAsync<string>("activeNavClient", clientId);
		}
	}
}
