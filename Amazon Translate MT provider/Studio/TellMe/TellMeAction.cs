using Sdl.TellMe.ProviderApi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Studio.TellMe
{
    public abstract class TellMeAction : AbstractTellMeAction
	{
		readonly string[] _helpKeywords = { "amazon", "provider", "translate", "amz", "aws", "machine translation" };
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

            if (!string.IsNullOrEmpty(_url))
            {
                Process.Start(_url);
            }
        }
    }
}