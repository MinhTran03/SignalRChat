using Microsoft.AspNetCore.Components;
using SignalRChat.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRChat.Clients.Blazor.Components
{
	public class ManySelectListPopupBase : ComponentBase
	{
		[Parameter] public List<ClientIdentity> ClientList { get; set; }
		[Parameter] public EventCallback<List<string>> OnCreateCallback { get; set; }
		protected List<string> SelectedClientIdList { get; set; } = new List<string>();

		protected void HandleCheckboxClicked(string checkedClientId, object checkedValue)
		{
			if ((bool)checkedValue)
			{
				SelectedClientIdList.Add(checkedClientId);
			}
			else
			{
				SelectedClientIdList.Remove(checkedClientId);
			}
		}

		protected async Task HandleCreate()
		{
			await OnCreateCallback.InvokeAsync(SelectedClientIdList);
		}
	}
}
