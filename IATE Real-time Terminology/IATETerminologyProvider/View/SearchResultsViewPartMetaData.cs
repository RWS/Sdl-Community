using Sdl.Community.IATETerminologyProvider.Helpers;
using System;
using System.Collections.Generic;
using TradosStudio.API.UI.View;

namespace Sdl.Community.IATETerminologyProvider.View
{
    internal class SearchResultsViewPartMetaData : IViewPartMetaData
    {
        public string Id => Constants.SearchResultsViewPartMetaDataId;

        public string Title => PluginResources.SearchResultsViewPartMetaData_Title;

        public string Description => PluginResources.SearchResultsViewPartMetaData_Description;

        public string Icon => $"Sdl.Community.IATETerminologyProvider.Resources.{nameof(PluginResources.Iate_logo)}.ico";

        public ITargetView TargetView => DefaultTargetViews.EditorView;

        public Type ViewPartType => typeof(SearchResultsViewPart);

        public IEnumerable<ViewPartLayoutInfo> Layouts => new List<ViewPartLayoutInfo>()
        {
            new ViewPartLayoutInfo()
            {
                Dock = DockType.Bottom,
                Width = 300,
                Height = 300,
                Visible = true,
                Pinned = true,
            }
        };
    }
}
