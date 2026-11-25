using Sdl.Community.IATETerminologyProvider.Helpers;
using System;
using System.Collections.Generic;
using TradosStudio.API.UI;
using TradosStudio.API.UI.Action;

namespace Sdl.Community.IATETerminologyProvider.Actions
{
    internal class IATESearchAllActionMetaData : IActionMetaData
    {
        public string Id => Constants.IATESearchAllActionId;

        public string Text => PluginResources.IATESearchAllAction_Name;

        public string TooltipText => string.Empty;

        public string Icon => $"Sdl.Community.IATETerminologyProvider.Resources.{nameof(PluginResources.Iate_logo)}.ico";

        public ActionSize Size => ActionSize.Default;

        public TargetSiteType TargetSiteType => TargetSiteType.View;

        public string TargetSiteId => "EditorView";

        public string Category => "Editor";

        public Type ActionType => typeof(IATESearchAllAction);

        public IEnumerable<IActionLocation> Locations => new List<IActionLocation>()
        {
            new RibbonGroupLocation(Constants.IATETerminologyProviderRibbonGroupId),
            new ContextMenuLocation(ActionLocationTargets.DocumentContextMenu)
        };

        public Keys ShortcutKeys => Keys.Alt | Keys.L;//necc
    }

    internal class IATESearchAllAction : ActionBase, IAction
    {
        public void OnInit()
        {
            
        }

        void IAction.Execute()
        {
            IATESearchActionHelper.NavigateToIATE(true);
        }
    }
}
