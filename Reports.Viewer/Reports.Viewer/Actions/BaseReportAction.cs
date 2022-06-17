using Sdl.Desktop.IntegrationApi;

namespace Reports.Viewer.Plus.Actions
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
