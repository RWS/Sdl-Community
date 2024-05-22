using Sdl.Community.XmlReader.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XmlReader.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
            new DocumentationAction
            {
                Keywords = new[] { "reader", "xml reader", "xmlreader", "community", "support", "wiki" }
            },
            new AppStoreForumAction
            {
                Keywords = new[] { "reader", "xml reader", "xmlreader", "support", "forum" }
            },
            new SourceCodeAction
            {
                Keywords = new[] { "reader", "xml reader", "xmlreader", "source code" }
            },
            new XmlReaderAction
            {
                Keywords = new[] { "reader", "xml reader", "xmlreader" }
            }
        };
	}
}
