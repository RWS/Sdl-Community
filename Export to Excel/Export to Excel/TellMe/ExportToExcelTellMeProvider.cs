using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportToExcel.TellMe
{
	[TellMeProvider]
	public class ExportToExcelTellMeProvider : ITellMeProvider
	{
		public string Name => "ExportToExcel Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new StoreAction()
			{
				Keywords = new []{"export to excel", "export to excel store", "export to excel download" }
			},
			new HelpAction()
			{
				Keywords = new []{ "export to excel", "export to excel help", "export to excel guide" }
			},
			new CommunityForumAction()
			{
				Keywords = new []{ "export to excel", "export to excel forum", "export to excel report" }
			}
		};
	}
}
