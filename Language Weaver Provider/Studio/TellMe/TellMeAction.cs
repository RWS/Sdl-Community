using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using LanguageWeaverProvider.Extensions;
using Sdl.TellMe.ProviderApi;

namespace LanguageWeaverProvider.Studio.TellMe
{
	public abstract class TellMeAction : AbstractTellMeAction
	{
		readonly string[] _helpKeywords = { "lw", "cloud", "edge", "language", "weaver", "machine translation" };
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
}