using Sdl.TellMe.ProviderApi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Sdl.Community.ExportToExcel.TellMe
{
    public abstract class TellMeAction : AbstractTellMeAction
    {
        readonly string[] _helpKeywords = ["export", "to", "excel", "batch", "task"];
        readonly string _url;
        readonly Action _customAction;

        protected TellMeAction(string name, Icon icon, string[] helpKeywords, bool isAvailable, string url = null, Action customAction = null)
        {
            _url = url;
            _customAction = customAction;

            Name = name;
            Icon = icon;
            IsAvailable = isAvailable;
            Keywords = [.. _helpKeywords, .. helpKeywords];
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

            Process.Start(_url);
        }
    }
}