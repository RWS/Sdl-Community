using System;
using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel
{
	public class QeFileReport
	{
		public LanguageDirection LanguageDirection { get; set; }
		public string FileName { get; set; }
		public Dictionary<string, (List<ISegmentPair>, CountData)> SegmentsPerCategory { get; set; } = new()
		{
			[PluginResources.GoodQuality] = Tuple.Create(new List<ISegmentPair>(), new CountData()).ToValueTuple(),
			[PluginResources.AdequateQuality] = Tuple.Create(new List<ISegmentPair>(), new CountData()).ToValueTuple(),
			[PluginResources.PoorQuality] = Tuple.Create(new List<ISegmentPair>(), new CountData()).ToValueTuple(),
			[PluginResources.UnknownQuality] = Tuple.Create(new List<ISegmentPair>(), new CountData()).ToValueTuple()
		};
	}
}