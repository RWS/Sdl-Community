using Sdl.TellMe.ProviderApi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Sdl.Community.TMLifting.Studio.TellMe
{
    public abstract class TellMeAction : AbstractTellMeAction
    {
        readonly string[] _helpKeywords = { "tm", "lifting" };
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

            if (!IsAvailable)
            {
                IsAvailable = GetAvailability();
            }
        }

        public override bool IsAvailable { get; }

        public override string Category => TellMeConstants.PluginName;

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

        private bool GetAvailability()
        {
            return Name switch
            {
                TellMeConstants.TellMe_FirstAction_Name => GetFirstActionAvailability(),
                TellMeConstants.TellMe_SecondAction_Name => GetSecondActionAvailability(),
                TellMeConstants.TellMe_ThirdAction_Name => GetThirdActionAvailability(),
                TellMeConstants.TellMe_MainView_Name => GetMainViewAvailability(),
                _ => false,
            };
        }

        private bool GetMainViewAvailability()
        {
            throw new NotImplementedException();
        }

        private bool GetThirdActionAvailability()
        {
            throw new NotImplementedException();
        }

        private bool GetSecondActionAvailability()
        {
            throw new NotImplementedException();
        }

        private bool GetFirstActionAvailability()
        {
            throw new NotImplementedException();
        }
    }
}