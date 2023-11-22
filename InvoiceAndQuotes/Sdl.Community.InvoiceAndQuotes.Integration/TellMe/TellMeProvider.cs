using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.InvoiceAndQuotes.Integration.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] {"invoice", "quote", "inquote", "invoiceandquote", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "invoice", "quote", "inquote", "invoiceandquote", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "invoice", "quote", "inquote", "invoiceandquote", "store", "download", "appstore" }}
		};
	}
}
