using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "Tados Export Analysis Reports Tell Me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new []{ "exportanalysisreports", "export analysis reports","export analysis reports documentation", "exportanalysisreports documentation",
					"report exporter", "report exporter guide", "report exporter documentation", "reportexporter", "reportexporter documentation", "reportexporter guide" }
			},
			new CommunitySupportAction
			{
				Keywords = new []{ "exportanalysisreports", "export analysis reports", "exportanalysisreports community", "exportanalysisreports support", "export analysis reports support",
					"export analysis reports community", "report exporter", "report exporter support", "report exporter community", "reportexporter", "reportexporter community", "reportexporter support" }
			},
			new SourceCodeAction
			{
				Keywords = new []{ "exportanalysisreports", "export analysis reports", "export analysis reports source code", "exportanalysisreports", "exportanalysisreports source code",
					 "report exporter", "reportexporter", "reportexporter source code", "report exporter source code" }
			},
			new SettingsAction
			{
				Keywords = new []{ "export", "analysis", "reports", "settings", "report", "exporter", "settings"}
			}
		};
	}
}
