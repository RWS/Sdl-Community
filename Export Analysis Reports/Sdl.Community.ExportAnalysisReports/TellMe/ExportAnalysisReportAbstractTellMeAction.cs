using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	public abstract class ExportAnalysisReportAbstractTellMeAction : AbstractTellMeAction
	{
		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override bool IsAvailable => true;
	}
}