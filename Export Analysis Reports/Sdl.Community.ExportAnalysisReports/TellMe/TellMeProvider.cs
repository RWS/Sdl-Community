using Sdl.Community.ExportAnalysisReports.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "Tados Export Analysis Reports Tell Me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction
			{
				Keywords = new []{ "exportanalysisreports", "export analysis reports", "exportanalysisreports community", "exportanalysisreports support", "export analysis reports support",
					"export analysis reports community", "report exporter", "report exporter support", "report exporter community", "reportexporter", "reportexporter community", "reportexporter support" }
			},			
			new HelpAction
			{
				Keywords = new []{ "exportanalysisreports", "export analysis reports", "exportanalysisreports help", "exportanalysisreports guide", "export analysis reports help",
					"export analysis reports guide", "report exporter", "report exporter guide", "report exporter help", "reportexporter", "reportexporter help", "reportexporter guide" }
			},
			new ExportAnalysisReportAction
			{
				Keywords = new []{ "exportanalysisreports", "export analysis reports", "settings"  }
			},
			new SourceCodeAction
			{
				Keywords = new []{ "exportanalysisreports", "export analysis reports", "source code"  }
			}
		};
	}
}
