using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.Actions
{
	public class BaseReportAction: AbstractAction
	{
		protected override void Execute()
		{			
		}

		public virtual void UpdateEnabled(bool loading)
		{
			Enabled = loading;
		}
	}
}
