using Sdl.TellMe.ProviderApi;

namespace ExportAnalysisReports.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "ExportAnalysisReports tell me provider";
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
			new StoreAction
			{
				Keywords = new []{ "exportanalysisreports", "export analysis reports", "exportanalysisreports store", "exportanalysisreports download", "export analysis reports store",
					"export analysis reports download", "report exporter", "reportexporter", "reportexporter store", "reportexporter download", "report exporter store", "report exporter download"  }
			}
		};
	}
}
