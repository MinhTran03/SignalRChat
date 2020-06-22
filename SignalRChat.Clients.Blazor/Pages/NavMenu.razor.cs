using Microsoft.AspNetCore.Components;

namespace SignalRChat.Clients.Blazor.Pages
{
	public class NavMenuBase : ComponentBase
	{
		protected bool collapseNavMenu = true;
		protected string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
		protected void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;


	}
}
