using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TermExcelerator.TellMe
{
	[TellMeProvider]
	public class ExcelTermTellMeProvider : ITellMeProvider
	{
		public string Name => "Term Excelerator Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new ExcelTermCommunityWikiAction
			{
				Keywords = new[] {"excel terminology", "excel terminology community", "excel terminology support", "excel terminology wiki",
								  "term excelerator", "term excelerator community", "term excelerator support", "term excelerator wiki"}
			},
			new ExcelTermCommunityForumAction
			{
				Keywords = new[] { "excel terminology", "excel terminology community", "excel terminology support", "excel terminology forum",
								   "term excelerator", "term excelerator community", "term excelerator support", "term excelerator forum"}
			},
			new ExcelTermStoreAction
			{
				Keywords = new[] { "excel terminology", "excel terminology store", "excel terminology download", "excel terminology appstore",
								   "term excelerator", "term excelerator store", "term excelerator download", "term excelerator appstore"}
			}
		};
	}
}