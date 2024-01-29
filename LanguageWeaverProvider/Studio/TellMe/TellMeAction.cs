using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using LanguageWeaverProvider.Extensions;
using Sdl.TellMe.ProviderApi;

namespace LanguageWeaverProvider.Studio.TellMe
{
	public abstract class TellMeAction : AbstractTellMeAction
	{
		readonly string[] _helpKeywords = { "lw", "cloud", "edge", "language weaver", "machine translation" };
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

		public override string Category => Constants.PluginName;

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

			try
			{
				Process.Start(_url);
			}
			catch (Exception ex)
			{
				ex?.ShowDialog("URL couldn't be oppened", ex.Message, true);
			}
		}
	}

	class HelpAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "help", "guide" };
		private static readonly bool _isAvailable = true;

		public HelpAction() : base(Constants.TellMe_Help_Name, PluginResources.Question, _helpKeywords, _isAvailable, url: Constants.TellMe_Help_Url) { }
	}

	class CommunityForumAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "community", "support" };
		private static readonly bool _isAvailable = true;
		public CommunityForumAction() : base(Constants.TellMe_Forum_Name, PluginResources.Question, _helpKeywords, _isAvailable, url: Constants.TellMe_Forum_Url) { }
	}

	class SourceCodeAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "source code", "github" };
		private static readonly bool _isAvailable = true;
		public SourceCodeAction() : base(Constants.TellMe_SourceCode_Name, PluginResources.ForumIcon, _helpKeywords, _isAvailable, url: Constants.TellMe_SourceCode_Url) { }
	}

	class SettingsAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "settings" };
		private static readonly bool _isAvailable = true;
		public SettingsAction() : base(Constants.TellMe_Settings_Name, PluginResources.ForumIcon, _helpKeywords, _isAvailable, customAction: DisplayMessage) { }
		private static void DisplayMessage()
		{
			MessageBox.Show("See documentation for guidance.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}