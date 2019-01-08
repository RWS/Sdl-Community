using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace ExportToExcel.ExportToExcelTellMe
{
	[TellMeProvider]
	public class ExportToExcelTellMeProvider : ITellMeProvider
	{
		public string Name => "ExportToExcel tell me provider";

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
