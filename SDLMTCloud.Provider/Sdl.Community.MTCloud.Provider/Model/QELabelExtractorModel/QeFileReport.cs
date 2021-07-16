using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.AutomaticTasks;

namespace Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel
{
	public class QeFileReport
	{
		public LanguageDirection LanguageDirection { get; set; }
		public string FileName { get; set; }
		public Dictionary<string, List<ISegmentPair>> SegmentsPerCategory { get; set; } = new Dictionary<string, List<ISegmentPair>>
		{
			[PluginResources.GoodQuality] = new List<ISegmentPair>(),
			[PluginResources.AdequateQuality] = new List<ISegmentPair>(),
			[PluginResources.PoorQuality] = new List<ISegmentPair>()
		};
	}
}