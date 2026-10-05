using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradosStudio.API.UI.View;

namespace Sdl.Community.IATETerminologyProvider.View
{
    internal class SearchResultsViewPartMetaData : IViewPartMetaData
    {
        public string Id => "IATE Results Viewer";

        public string Title => "Search Results Viewer";

        public string Description => "IATE Search Results";

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
