using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
	public abstract class TellMeAction : AbstractTellMeAction
	{
		readonly string[] _helpKeywords = { "data", "protection", "suite", "dps", "plugin" };

		readonly string _url;
		readonly Action _customAction;

		protected TellMeAction(string name, Icon icon, string[] helpKeywords, bool isAvailable, string url = null, Action customAction = null)
		{
			_url = url;
			_customAction = customAction;

			Name = name;
			Icon = icon;
			IsAvailable = isAvailable;
			Keywords = _helpKeywords.Concat(helpKeywords).ToArray();
		}

		public override bool IsAvailable { get; }

		public override string Category => PluginResources.Plugin_Name;

		public override void Execute()
		{
			if (_customAction is not null)
			{
				_customAction.Invoke();
				return;
			}

			if (string.IsNullOrEmpty(_url))
			{
				return;
			}

			Process.Start(_url);
		}
	}

	class DocumentationAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "help", "guide" };
		private static readonly bool _isAvailable = true;

		public DocumentationAction() : base($"{PluginResources.Plugin_Name} Documentation", PluginResources.ForumIcon, _helpKeywords, _isAvailable, url: "https://appstore.rws.com/Plugin/39?tab=documentation") { }
	}

	class CommunityForumAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "community", "support", "documentation" };
		private static readonly bool _isAvailable = true;
		public CommunityForumAction() : base("RWS Community AppStore Forum", PluginResources.question, _helpKeywords, _isAvailable, url: "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f") { }
	}

	class SourceCodeAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "source code", "github" };
		private static readonly bool _isAvailable = true;
		public SourceCodeAction() : base($"{PluginResources.Plugin_Name} Source Code", PluginResources.ForumIcon, _helpKeywords, _isAvailable, url: "https://github.com/RWS/Sdl-Community/tree/master/SDLDataProtectionSuite") { }
	}

	class SettingsAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "settings" };
		private static readonly bool _isAvailable = true;
		public SettingsAction() : base($"{PluginResources.Plugin_Name} Settings", PluginResources.ForumIcon, _helpKeywords, _isAvailable, customAction: DisplayMessage) { }
		private static void DisplayMessage()
		{
			new SettingsActionView("https://appstore.rws.com/Plugin/39?tab=documentation").ShowDialog();
		}
	}
}