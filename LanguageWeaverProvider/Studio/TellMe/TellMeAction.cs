using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Sdl.TellMe.ProviderApi;

namespace LanguageWeaverProvider.Studio.TellMe
{
	public abstract class TellMeAction : AbstractTellMeAction
	{
		private readonly string[] _helpKeywords = { "lw", "cloud", "edge", "language weaver", "machine translation" };
		private readonly string _url;

		protected TellMeAction(string name, Icon icon, string url, string[] helpKeywords)
		{
			Name = name;
			Icon = icon;
			_url = url;
			Keywords = _helpKeywords.Concat(helpKeywords).ToArray();
		}

		public override bool IsAvailable => true;

		public override string Category => Constants.PluginName;

		public override void Execute()
		{
			if (string.IsNullOrEmpty(_url))
			{
				return;
			}

			Process.Start(_url);
		}
	}

	class HelpAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "help", "guide" };

		public HelpAction() : base(Constants.TellMe_Help_Name, PluginResources.Question, Constants.TellMe_Help_Url, _helpKeywords) { }
	}

	class CommunityForumAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "community", "support" };
		public CommunityForumAction() : base(Constants.TellMe_Forum_Name, PluginResources.ForumIcon, Constants.TellMe_Forum_Url, _helpKeywords) { }
	}

	class SourceCodeAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "source code", "github" };
		public SourceCodeAction() : base(Constants.TellMe_SourceCode_Name, PluginResources.ForumIcon, Constants.TellMe_SourceCode_Url, _helpKeywords) { }
	}
}