using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.DsiViewer.Model;
using Sdl.Community.DsiViewer.Studio.DisplayFilters;

namespace Sdl.Community.DsiViewer.Service
{
	public class FilterApplier
	{
		private SdlMtCloudDisplayFilter SdlMtCloudDisplayFilter { get; } = new();
		public SdlMtCloudFilterSettings SdlMtCloudFilterSettings => SdlMtCloudDisplayFilter.Settings;

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
