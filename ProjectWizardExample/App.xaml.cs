using System.Windows;

namespace ProjectWizardExample
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var startup = new Startup();
			startup.Execute();
		}
	}
}
