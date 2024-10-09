using Sdl.Community.DsiViewer.Model;
using Sdl.Community.DsiViewer.Studio.DisplayFilters;

namespace Sdl.Community.DsiViewer.Services
{
    public class FilterApplier
    {
        public SdlMtCloudFilterSettings SdlMtCloudFilterSettings => SdlMtCloudDisplayFilter.Settings;
        private SdlMtCloudDisplayFilter SdlMtCloudDisplayFilter { get; } = new();

        public void ApplyFilter()
        {
            DsiViewerInitializer.EditorController.ActiveDocument.ApplyFilterOnSegments(SdlMtCloudDisplayFilter);
        }

        public void ClearFilter()
        {
            SdlMtCloudFilterSettings.ClearFilter();
        }
    }
}