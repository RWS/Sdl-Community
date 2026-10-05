using Sdl.Community.IATETerminologyProvider.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TradosStudio.API.UI;
using TradosStudio.API.UI.Action;
using TradosStudio.API.UI.View;

namespace Sdl.Community.IATETerminologyProvider.Actions
{
    internal class IATESearchSourceLanguageActionMetaData : IActionMetaData
    {
        public string Id => Constants.IATESearchSourceLanguageActionId;

        public string Text => PluginResources.IATESearchSourceTargetAction_Name;

        public string TooltipText => string.Empty;

        public string Icon => $"Sdl.Community.IATETerminologyProvider.Resources.{nameof(PluginResources.Iate_logo)}.ico";

        public ActionSize Size => ActionSize.Default;

        public TargetSiteType TargetSiteType => TargetSiteType.View;

        public string TargetSiteId => "EditorView";

        public string Category => "Editor";

        public Type ActionType => typeof(IATESearchSourceLanguageAction);

        public IEnumerable<IActionLocation> Locations => new List<IActionLocation>() 
        {
            new RibbonGroupLocation(Constants.IATETerminologyProviderRibbonGroupId),
            new ContextMenuLocation(ActionLocationTargets.DocumentExplorerContextMenu)
        };

        public Keys ShortcutKeys => Keys.Alt | Keys.L;//necc
    }

    internal class IATESearchSourceLanguageAction : ActionBase, IAction
    {
        private IView _view;

        public IATESearchSourceLanguageAction(IEnumerable<IView> views)
        {
            _view = views.ToList().FirstOrDefault();
        }

        public void OnInit()
        {        
            
        }

        void IAction.Execute()
        {
            IATESearchActionHelper.NavigateToIATE(false, _view);
        }
    }
}
